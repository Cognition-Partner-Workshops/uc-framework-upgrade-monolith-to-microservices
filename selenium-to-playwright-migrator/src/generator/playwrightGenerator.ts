import { ParsedJavaFile, MigrationResult } from '../types';
import { ApiMapper } from '../mapper/apiMapper';
import {
  classNameToTestFile,
  methodNameToTestDescription,
  indent,
} from '../utils/helpers';

/**
 * Generates TypeScript Playwright test code from parsed Selenium Java test files.
 */
export class PlaywrightGenerator {
  private mapper: ApiMapper;

  constructor() {
    this.mapper = new ApiMapper();
  }

  /**
   * Generate a complete Playwright test file from a parsed Java file
   */
  generate(parsed: ParsedJavaFile, inputFile: string): MigrationResult {
    this.mapper.resetDiagnostics();

    const outputFile = classNameToTestFile(parsed.className || 'UnknownTest');
    const lines: string[] = [];

    // Generate imports
    lines.push(...this.generateImports(parsed));
    lines.push('');

    // Generate test.describe block
    const descName = parsed.className.replace(/Test$/i, '');
    lines.push(`test.describe('${descName}', () => {`);

    // Generate beforeEach from setup method
    if (parsed.setupMethod) {
      lines.push(...this.generateHook('beforeEach', parsed.setupMethod));
      lines.push('');
    }

    // Generate afterEach from teardown method
    if (parsed.teardownMethod) {
      lines.push(...this.generateHook('afterEach', parsed.teardownMethod));
      lines.push('');
    }

    // Generate test methods
    for (const testMethod of parsed.testMethods) {
      lines.push(...this.generateTestMethod(testMethod));
      lines.push('');
    }

    // Generate non-test, non-lifecycle helper methods
    for (const method of parsed.methods) {
      if (!method.isTest && !method.isSetup && !method.isTeardown) {
        lines.push(...this.generateHelperMethod(method));
        lines.push('');
      }
    }

    // Close describe block
    lines.push('});');
    lines.push('');

    const generatedCode = lines.join('\n');

    return {
      inputFile,
      outputFile,
      generatedCode,
      warnings: this.mapper.getWarnings(),
      unsupportedPatterns: this.mapper.getUnsupportedPatterns(),
    };
  }

  /**
   * Generate import statements for the Playwright test file
   */
  private generateImports(parsed: ParsedJavaFile): string[] {
    const imports: string[] = [];
    imports.push("import { test, expect } from '@playwright/test';");

    // Check if Page type is needed (for helper methods with page parameter)
    const needsPageType = parsed.methods.some(
      (m) => !m.isTest && !m.isSetup && !m.isTeardown
    );
    if (needsPageType) {
      imports.push("import { Page } from '@playwright/test';");
    }

    return imports;
  }

  /**
   * Generate a beforeEach/afterEach hook
   */
  private generateHook(
    hookName: string,
    method: { body: string[]; name: string }
  ): string[] {
    const lines: string[] = [];
    // For teardown hooks, Playwright handles cleanup automatically
    if (hookName === 'afterEach') {
      lines.push(`  // Playwright handles browser cleanup automatically via test fixtures`);
      return lines;
    }

    lines.push(`  test.${hookName}(async ({ page }) => {`);

    const bodyLines = this.transformMethodBody(method.body);
    for (const line of bodyLines) {
      if (line.trim()) {
        lines.push(`    ${line}`);
      }
    }

    lines.push('  });');
    return lines;
  }

  /**
   * Generate a test method
   */
  private generateTestMethod(method: {
    name: string;
    body: string[];
    annotations: string[];
  }): string[] {
    const lines: string[] = [];
    const testDescription = methodNameToTestDescription(method.name);

    lines.push(
      `  test('${testDescription}', async ({ page }) => {`
    );

    const bodyLines = this.transformMethodBody(method.body);
    for (const line of bodyLines) {
      if (line.trim()) {
        lines.push(`    ${line}`);
      }
    }

    lines.push('  });');
    return lines;
  }

  /**
   * Generate a helper/utility method
   */
  private generateHelperMethod(method: {
    name: string;
    body: string[];
    parameters: { type: string; name: string }[];
    returnType: string;
  }): string[] {
    const lines: string[] = [];

    // Build parameter list, adding page parameter
    const params = ['page: Page'];
    for (const param of method.parameters) {
      if (
        param.type !== 'WebDriver' &&
        param.name !== 'driver'
      ) {
        const tsType = this.mapJavaTypeToTS(param.type);
        params.push(`${param.name}: ${tsType}`);
      }
    }

    const returnType = this.mapJavaTypeToTS(method.returnType);
    const asyncPrefix = 'async ';

    lines.push(
      `  ${asyncPrefix}function ${method.name}(${params.join(', ')}): Promise<${returnType}> {`
    );

    const bodyLines = this.transformMethodBody(method.body);
    for (const line of bodyLines) {
      if (line.trim()) {
        lines.push(`    ${line}`);
      }
    }

    lines.push('  }');
    return lines;
  }

  /**
   * Transform a method body from Selenium Java to Playwright TypeScript
   */
  private transformMethodBody(bodyLines: string[]): string[] {
    const result: string[] = [];

    for (const line of bodyLines) {
      const trimmed = line.trim();

      // Skip empty lines
      if (!trimmed) {
        result.push('');
        continue;
      }

      // Skip Java-specific constructs that don't apply
      if (this.shouldSkipLine(trimmed)) {
        continue;
      }

      // Convert Java .contains() to .includes() within the line
      const processedLine = line.replace(/\.contains\(/g, '.includes(');

      // Handle try-catch blocks
      if (trimmed === 'try {') {
        result.push('try {');
        continue;
      }
      if (trimmed.startsWith('} catch')) {
        const catchMatch = trimmed.match(/\}\s*catch\s*\((\w+)\s+(\w+)\)\s*\{/);
        if (catchMatch) {
          result.push(`} catch (${catchMatch[2]}: unknown) {`);
        } else {
          result.push('} catch (error: unknown) {');
        }
        continue;
      }
      if (trimmed === '} finally {') {
        result.push('} finally {');
        continue;
      }
      if (trimmed === '}') {
        result.push('}');
        continue;
      }

      // Handle for loops
      const forMatch = trimmed.match(
        /for\s*\((\w+)\s+(\w+)\s*:\s*(.+)\)\s*\{/
      );
      if (forMatch) {
        result.push(`for (const ${forMatch[2]} of ${forMatch[3]}) {`);
        continue;
      }

      // Handle traditional for loops
      const traditionalForMatch = trimmed.match(
        /for\s*\(int\s+(\w+)\s*=\s*(\d+);\s*\w+\s*(<|<=|>|>=)\s*(.+?);\s*\w+(\+\+|--)\)\s*\{/
      );
      if (traditionalForMatch) {
        result.push(
          `for (let ${traditionalForMatch[1]} = ${traditionalForMatch[2]}; ${traditionalForMatch[1]} ${traditionalForMatch[3]} ${traditionalForMatch[4]}; ${traditionalForMatch[1]}${traditionalForMatch[5]}) {`
        );
        continue;
      }

      // Handle if/else
      if (trimmed.startsWith('if ') || trimmed.startsWith('if(')) {
        result.push(trimmed.replace(/;$/, ''));
        continue;
      }
      if (trimmed.startsWith('} else if') || trimmed.startsWith('} else {') || trimmed === 'else {') {
        result.push(trimmed);
        continue;
      }

      // Map the line using the API mapper
      const mapped = this.mapper.mapLine(processedLine);
      result.push(mapped.code);
    }

    return result;
  }

  /**
   * Check if a line should be skipped during migration
   */
  private shouldSkipLine(line: string): boolean {
    // Skip Java-specific lines that have no TypeScript equivalent
    const skipPatterns = [
      /^import\s/,
      /^package\s/,
      /^@(Override|SuppressWarnings)/,
      /^public\s+class\s/,
      /^private\s+static\s+final\s+Logger/,
      /^\s*logger\.(info|debug|warn|error)\(/,
      /^if\s*\(\s*\w+\s*!=\s*null\s*\)/,  // Skip null checks for driver variables
    ];
    return skipPatterns.some((p) => p.test(line));
  }

  /**
   * Map a Java type to TypeScript type
   */
  private mapJavaTypeToTS(javaType: string): string {
    const typeMap: Record<string, string> = {
      void: 'void',
      String: 'string',
      int: 'number',
      Integer: 'number',
      long: 'number',
      Long: 'number',
      double: 'number',
      Double: 'number',
      float: 'number',
      Float: 'number',
      boolean: 'boolean',
      Boolean: 'boolean',
      Object: 'unknown',
      WebElement: 'Locator',
      WebDriver: 'Page',
    };
    return typeMap[javaType] || javaType;
  }
}
