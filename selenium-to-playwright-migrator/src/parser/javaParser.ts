import {
  ParsedJavaFile,
  FieldDeclaration,
  MethodDeclaration,
  Parameter,
} from '../types';
import { isSeleniumImport, isTestFrameworkImport } from '../utils/helpers';

/**
 * Parses a Selenium Java test file into a structured representation.
 * Uses regex-based line-by-line parsing to extract class structure,
 * fields, methods, annotations, and Selenium API calls.
 */
export class JavaParser {
  private lines: string[] = [];
  private currentLine = 0;

  parse(sourceCode: string): ParsedJavaFile {
    this.lines = sourceCode.split('\n');
    this.currentLine = 0;

    const result: ParsedJavaFile = {
      packageName: '',
      imports: [],
      className: '',
      baseClass: null,
      fields: [],
      methods: [],
      annotations: [],
      setupMethod: null,
      teardownMethod: null,
      testMethods: [],
    };

    this.parsePackageAndImports(result);
    this.parseClassBody(result);
    this.categorizeMethodsByAnnotation(result);

    return result;
  }

  private parsePackageAndImports(result: ParsedJavaFile): void {
    for (const line of this.lines) {
      const trimmed = line.trim();

      const packageMatch = trimmed.match(/^package\s+([\w.]+)\s*;/);
      if (packageMatch) {
        result.packageName = packageMatch[1];
        continue;
      }

      const importMatch = trimmed.match(/^import\s+([\w.*]+)\s*;/);
      if (importMatch) {
        result.imports.push(importMatch[1]);
      }
    }
  }

  private parseClassBody(result: ParsedJavaFile): void {
    let insideClass = false;
    let braceDepth = 0;
    let currentAnnotations: string[] = [];
    let methodBuffer: string[] = [];
    let collectingMethod = false;
    let methodStartAnnotations: string[] = [];
    let methodSignature = '';

    for (let i = 0; i < this.lines.length; i++) {
      const line = this.lines[i];
      const trimmed = line.trim();

      // Parse class declaration
      if (!insideClass) {
        const classMatch = trimmed.match(
          /(?:public\s+)?class\s+(\w+)(?:\s+extends\s+(\w+))?(?:\s+implements\s+[\w,\s]+)?\s*\{/
        );
        if (classMatch) {
          result.className = classMatch[1];
          result.baseClass = classMatch[2] || null;
          insideClass = true;
          braceDepth = 1;
          continue;
        }
        continue;
      }

      // Track annotations
      if (trimmed.startsWith('@')) {
        currentAnnotations.push(trimmed);
        continue;
      }

      // Skip empty lines between annotations
      if (trimmed === '' && currentAnnotations.length > 0) {
        continue;
      }

      // Parse field declarations (not inside a method)
      if (!collectingMethod && this.isFieldDeclaration(trimmed)) {
        const field = this.parseFieldDeclaration(trimmed, currentAnnotations);
        if (field) {
          result.fields.push(field);
        }
        currentAnnotations = [];
        continue;
      }

      // Parse method declarations
      if (!collectingMethod && this.isMethodDeclaration(trimmed)) {
        collectingMethod = true;
        methodStartAnnotations = [...currentAnnotations];
        currentAnnotations = [];
        methodSignature = trimmed;
        methodBuffer = [];

        // Count braces in the signature line
        for (const ch of trimmed) {
          if (ch === '{') braceDepth++;
          if (ch === '}') braceDepth--;
        }

        // If method is a one-liner with closing brace
        if (trimmed.includes('{') && trimmed.includes('}') && braceDepth <= 1) {
          const method = this.buildMethod(
            methodSignature,
            methodBuffer,
            methodStartAnnotations
          );
          if (method) {
            result.methods.push(method);
          }
          collectingMethod = false;
          braceDepth = 1;
        }
        continue;
      }

      if (collectingMethod) {
        for (const ch of trimmed) {
          if (ch === '{') braceDepth++;
          if (ch === '}') braceDepth--;
        }

        if (braceDepth <= 1) {
          // Method ended
          const method = this.buildMethod(
            methodSignature,
            methodBuffer,
            methodStartAnnotations
          );
          if (method) {
            result.methods.push(method);
          }
          collectingMethod = false;
          braceDepth = 1;
        } else {
          methodBuffer.push(line);
        }
        continue;
      }

      currentAnnotations = [];
    }
  }

  private isFieldDeclaration(line: string): boolean {
    // Match field declarations like: private WebDriver driver;
    // or: private WebDriver driver = new ChromeDriver();
    return /^\s*(?:private|protected|public|static|final|\s)+\s+\w+(?:<[\w,\s<>]+>)?\s+\w+\s*(?:=.*)?;/.test(
      line
    );
  }

  private parseFieldDeclaration(
    line: string,
    annotations: string[]
  ): FieldDeclaration | null {
    const match = line.match(
      /(?:private|protected|public|static|final|\s)+\s+(\w+(?:<[\w,\s<>]+>)?)\s+(\w+)\s*(?:=\s*(.+?))?;/
    );
    if (!match) return null;

    return {
      type: match[1],
      name: match[2],
      initializer: match[3]?.trim() || null,
      annotations,
    };
  }

  private isMethodDeclaration(line: string): boolean {
    return /^\s*(?:public|private|protected|static|\s)*\s*(?:void|String|int|boolean|WebDriver|WebElement|List|Map|\w+)\s+\w+\s*\(/.test(
      line
    );
  }

  private buildMethod(
    signature: string,
    bodyLines: string[],
    annotations: string[]
  ): MethodDeclaration | null {
    const sigMatch = signature.match(
      /(?:public|private|protected|static|\s)*\s*(\w+(?:<[\w,\s<>]+>)?)\s+(\w+)\s*\(([^)]*)\)/
    );
    if (!sigMatch) return null;

    const returnType = sigMatch[1];
    const name = sigMatch[2];
    const paramsStr = sigMatch[3];

    const parameters: Parameter[] = [];
    if (paramsStr.trim()) {
      const paramParts = paramsStr.split(',');
      for (const param of paramParts) {
        const paramMatch = param.trim().match(/(\w+(?:<[\w,\s<>]+>)?)\s+(\w+)/);
        if (paramMatch) {
          parameters.push({ type: paramMatch[1], name: paramMatch[2] });
        }
      }
    }

    const isTest =
      annotations.some(
        (a) =>
          a === '@Test' ||
          a.startsWith('@Test(') ||
          a.startsWith('@org.junit') ||
          a.startsWith('@org.testng')
      ) || name.startsWith('test');

    const isSetup = annotations.some(
      (a) =>
        a === '@Before' ||
        a === '@BeforeMethod' ||
        a === '@BeforeEach' ||
        a === '@BeforeClass' ||
        a === '@BeforeAll' ||
        a === '@Setup'
    );

    const isTeardown = annotations.some(
      (a) =>
        a === '@After' ||
        a === '@AfterMethod' ||
        a === '@AfterEach' ||
        a === '@AfterClass' ||
        a === '@AfterAll' ||
        a === '@TearDown'
    );

    return {
      name,
      returnType,
      parameters,
      body: bodyLines,
      annotations,
      isTest,
      isSetup,
      isTeardown,
    };
  }

  private categorizeMethodsByAnnotation(result: ParsedJavaFile): void {
    for (const method of result.methods) {
      if (method.isSetup) {
        result.setupMethod = method;
      } else if (method.isTeardown) {
        result.teardownMethod = method;
      }
      if (method.isTest) {
        result.testMethods.push(method);
      }
    }
  }
}
