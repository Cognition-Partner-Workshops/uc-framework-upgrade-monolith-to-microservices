/**
 * Represents a parsed Selenium Java test file
 */
export interface ParsedJavaFile {
  packageName: string;
  imports: string[];
  className: string;
  baseClass: string | null;
  fields: FieldDeclaration[];
  methods: MethodDeclaration[];
  annotations: string[];
  setupMethod: MethodDeclaration | null;
  teardownMethod: MethodDeclaration | null;
  testMethods: MethodDeclaration[];
}

export interface FieldDeclaration {
  type: string;
  name: string;
  initializer: string | null;
  annotations: string[];
}

export interface MethodDeclaration {
  name: string;
  returnType: string;
  parameters: Parameter[];
  body: string[];
  annotations: string[];
  isTest: boolean;
  isSetup: boolean;
  isTeardown: boolean;
}

export interface Parameter {
  type: string;
  name: string;
}

/**
 * Represents a mapped Playwright action
 */
export interface PlaywrightAction {
  type: ActionType;
  code: string;
  comment?: string;
  needsAwait: boolean;
}

export enum ActionType {
  NAVIGATION = 'navigation',
  LOCATOR = 'locator',
  ACTION = 'action',
  ASSERTION = 'assertion',
  WAIT = 'wait',
  BROWSER_MANAGEMENT = 'browser_management',
  VARIABLE_DECLARATION = 'variable_declaration',
  CONTROL_FLOW = 'control_flow',
  COMMENT = 'comment',
  UNKNOWN = 'unknown',
}

/**
 * Migration result for a single file
 */
export interface MigrationResult {
  inputFile: string;
  outputFile: string;
  generatedCode: string;
  warnings: string[];
  unsupportedPatterns: string[];
}

/**
 * Configuration options for the migrator
 */
export interface MigratorConfig {
  inputDir: string;
  outputDir: string;
  useTestFixtures: boolean;
  generatePageObjects: boolean;
  verbose: boolean;
}

/**
 * Selenium locator strategy mapping
 */
export interface LocatorMapping {
  seleniumMethod: string;
  playwrightEquivalent: string;
  transformer: (value: string) => string;
}
