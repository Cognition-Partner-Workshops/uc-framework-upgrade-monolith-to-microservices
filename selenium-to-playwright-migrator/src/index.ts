#!/usr/bin/env node

import { Command } from 'commander';
import * as fs from 'fs';
import * as path from 'path';
import { JavaParser } from './parser/javaParser';
import { PlaywrightGenerator } from './generator/playwrightGenerator';
import { MigrationResult, MigratorConfig } from './types';

const program = new Command();

program
  .name('sel2pw')
  .description('Migrate Selenium Java test code to TypeScript Playwright')
  .version('1.0.0');

program
  .command('migrate')
  .description('Migrate Selenium Java test files to Playwright TypeScript')
  .requiredOption('-i, --input <path>', 'Input directory or file containing Selenium Java tests')
  .requiredOption('-o, --output <path>', 'Output directory for generated Playwright tests')
  .option('--verbose', 'Show detailed migration output', false)
  .action((options) => {
    const config: MigratorConfig = {
      inputDir: path.resolve(options.input),
      outputDir: path.resolve(options.output),
      useTestFixtures: true,
      generatePageObjects: false,
      verbose: options.verbose,
    };

    runMigration(config);
  });

program
  .command('analyze')
  .description('Analyze Selenium Java files and report migration complexity')
  .requiredOption('-i, --input <path>', 'Input directory or file to analyze')
  .action((options) => {
    const inputPath = path.resolve(options.input);
    analyzeFiles(inputPath);
  });

function runMigration(config: MigratorConfig): void {
  console.log('\n=== Selenium to Playwright Migration ===\n');

  const javaFiles = collectJavaFiles(config.inputDir);

  if (javaFiles.length === 0) {
    console.error(`No Java files found in: ${config.inputDir}`);
    process.exit(1);
  }

  console.log(`Found ${javaFiles.length} Java file(s) to migrate.\n`);

  // Ensure output directory exists
  if (!fs.existsSync(config.outputDir)) {
    fs.mkdirSync(config.outputDir, { recursive: true });
  }

  const parser = new JavaParser();
  const generator = new PlaywrightGenerator();
  const results: MigrationResult[] = [];

  for (const javaFile of javaFiles) {
    console.log(`Migrating: ${path.basename(javaFile)}`);

    try {
      const sourceCode = fs.readFileSync(javaFile, 'utf-8');
      const parsed = parser.parse(sourceCode);
      const result = generator.generate(parsed, javaFile);

      // Write output file
      const outputPath = path.join(config.outputDir, result.outputFile);
      fs.writeFileSync(outputPath, result.generatedCode, 'utf-8');

      results.push(result);

      console.log(`  -> ${result.outputFile}`);

      if (result.warnings.length > 0) {
        console.log(`  Warnings: ${result.warnings.length}`);
        if (config.verbose) {
          result.warnings.forEach((w) => console.log(`    - ${w}`));
        }
      }

      if (result.unsupportedPatterns.length > 0) {
        console.log(`  Unsupported patterns: ${result.unsupportedPatterns.length}`);
        if (config.verbose) {
          result.unsupportedPatterns.forEach((p) =>
            console.log(`    - ${p}`)
          );
        }
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : String(error);
      console.error(`  ERROR: Failed to migrate ${javaFile}: ${errorMessage}`);
    }
  }

  // Generate playwright.config.ts if it doesn't exist
  const configPath = path.join(config.outputDir, 'playwright.config.ts');
  if (!fs.existsSync(configPath)) {
    generatePlaywrightConfig(config.outputDir);
    console.log('\nGenerated playwright.config.ts');
  }

  // Print summary
  printSummary(results);
}

function analyzeFiles(inputPath: string): void {
  console.log('\n=== Selenium Test Analysis ===\n');

  const javaFiles = collectJavaFiles(inputPath);

  if (javaFiles.length === 0) {
    console.error(`No Java files found in: ${inputPath}`);
    process.exit(1);
  }

  const parser = new JavaParser();
  let totalTests = 0;
  let totalMethods = 0;
  const allImports = new Set<string>();

  for (const javaFile of javaFiles) {
    const sourceCode = fs.readFileSync(javaFile, 'utf-8');
    const parsed = parser.parse(sourceCode);

    console.log(`File: ${path.basename(javaFile)}`);
    console.log(`  Class: ${parsed.className}`);
    console.log(`  Base class: ${parsed.baseClass || 'none'}`);
    console.log(`  Test methods: ${parsed.testMethods.length}`);
    console.log(`  Helper methods: ${parsed.methods.length - parsed.testMethods.length}`);
    console.log(`  Fields: ${parsed.fields.length}`);
    console.log(`  Has setup: ${parsed.setupMethod ? 'yes' : 'no'}`);
    console.log(`  Has teardown: ${parsed.teardownMethod ? 'yes' : 'no'}`);

    totalTests += parsed.testMethods.length;
    totalMethods += parsed.methods.length;
    parsed.imports.forEach((imp) => allImports.add(imp));

    console.log('');
  }

  console.log('=== Summary ===');
  console.log(`Total files: ${javaFiles.length}`);
  console.log(`Total test methods: ${totalTests}`);
  console.log(`Total methods: ${totalMethods}`);
  console.log(`\nSelenium imports detected:`);
  allImports.forEach((imp) => {
    if (imp.includes('selenium') || imp.includes('openqa')) {
      console.log(`  - ${imp}`);
    }
  });
}

function collectJavaFiles(inputPath: string): string[] {
  const stats = fs.statSync(inputPath);

  if (stats.isFile() && inputPath.endsWith('.java')) {
    return [inputPath];
  }

  if (stats.isDirectory()) {
    return collectJavaFilesRecursive(inputPath);
  }

  return [];
}

function collectJavaFilesRecursive(dir: string): string[] {
  const files: string[] = [];
  const entries = fs.readdirSync(dir, { withFileTypes: true });

  for (const entry of entries) {
    const fullPath = path.join(dir, entry.name);
    if (entry.isDirectory()) {
      files.push(...collectJavaFilesRecursive(fullPath));
    } else if (entry.name.endsWith('.java')) {
      files.push(fullPath);
    }
  }

  return files;
}

function generatePlaywrightConfig(outputDir: string): void {
  const config = `import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',
  use: {
    baseURL: 'http://localhost:3000', // TODO: Update with your application URL
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },
    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'] },
    },
  ],
});
`;
  fs.writeFileSync(path.join(outputDir, 'playwright.config.ts'), config, 'utf-8');
}

function printSummary(results: MigrationResult[]): void {
  console.log('\n=== Migration Summary ===\n');
  console.log(`Files migrated: ${results.length}`);

  const totalWarnings = results.reduce((sum, r) => sum + r.warnings.length, 0);
  const totalUnsupported = results.reduce(
    (sum, r) => sum + r.unsupportedPatterns.length,
    0
  );

  console.log(`Total warnings: ${totalWarnings}`);
  console.log(`Unsupported patterns: ${totalUnsupported}`);

  if (totalUnsupported > 0) {
    console.log(
      '\nSearch for "TODO: Manually migrate" in the output files for items that need manual attention.'
    );
  }

  console.log('\nNext steps:');
  console.log('  1. Review the generated test files');
  console.log('  2. Install Playwright: npm init playwright@latest');
  console.log('  3. Update playwright.config.ts with your app URL');
  console.log('  4. Run tests: npx playwright test');
  console.log('');
}

program.parse();
