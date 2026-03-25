/**
 * Utility helpers for the Selenium to Playwright migrator
 */

/**
 * Convert a Java class name to a Playwright test file name
 * e.g., "LoginPageTest" -> "login-page.spec.ts"
 */
export function classNameToTestFile(className: string): string {
  const withoutTest = className.replace(/Test$/i, '');
  const kebabCase = withoutTest
    .replace(/([A-Z])/g, '-$1')
    .toLowerCase()
    .replace(/^-/, '');
  return `${kebabCase}.spec.ts`;
}

/**
 * Convert a Java method name to a descriptive test name
 * e.g., "testLoginWithValidCredentials" -> "login with valid credentials"
 */
export function methodNameToTestDescription(methodName: string): string {
  const withoutPrefix = methodName
    .replace(/^test_?/i, '')
    .replace(/^should_?/i, 'should ');
  return withoutPrefix
    .replace(/([A-Z])/g, ' $1')
    .replace(/_/g, ' ')
    .toLowerCase()
    .trim();
}

/**
 * Convert Java string concatenation to TypeScript template literals
 * e.g., '"Hello " + name + "!"' -> '`Hello ${name}!`'
 */
export function convertStringConcatenation(javaString: string): string {
  if (!javaString.includes('+')) {
    return javaString;
  }

  const parts = javaString.split(/\s*\+\s*/);
  let result = '`';
  for (const part of parts) {
    const trimmed = part.trim();
    if (trimmed.startsWith('"') && trimmed.endsWith('"')) {
      result += trimmed.slice(1, -1);
    } else {
      result += '${' + trimmed + '}';
    }
  }
  result += '`';
  return result;
}

/**
 * Extract the string value from a Java string literal
 */
export function extractStringLiteral(value: string): string {
  const match = value.match(/^"(.*)"$/);
  return match ? match[1] : value;
}

/**
 * Indent a block of code
 */
export function indent(code: string, level: number = 1): string {
  const spaces = '  '.repeat(level);
  return code
    .split('\n')
    .map((line) => (line.trim() ? spaces + line : line))
    .join('\n');
}

/**
 * Convert Java type to TypeScript type
 */
export function javaTypeToTypeScript(javaType: string): string {
  const typeMap: Record<string, string> = {
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
    void: 'void',
    Object: 'any',
    List: 'Array',
    Map: 'Record',
    WebDriver: 'Page',
    WebElement: 'Locator',
    ChromeDriver: 'Browser',
    FirefoxDriver: 'Browser',
    EdgeDriver: 'Browser',
    RemoteWebDriver: 'Browser',
  };
  return typeMap[javaType] || javaType;
}

/**
 * Clean up a line by removing trailing semicolons and trimming
 */
export function cleanLine(line: string): string {
  return line.trim().replace(/;$/, '').trim();
}

/**
 * Check if a line is a Selenium import
 */
export function isSeleniumImport(line: string): boolean {
  return (
    line.includes('org.openqa.selenium') ||
    line.includes('org.seleniumhq')
  );
}

/**
 * Check if a line is a TestNG or JUnit import
 */
export function isTestFrameworkImport(line: string): boolean {
  return (
    line.includes('org.testng') ||
    line.includes('org.junit') ||
    line.includes('junit.framework')
  );
}
