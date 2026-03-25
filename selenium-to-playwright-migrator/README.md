# Selenium Java to TypeScript Playwright Migrator

A CLI tool that automatically migrates Selenium Java test code to TypeScript Playwright tests.

## Features

- **Java Parser**: Parses Selenium Java test files to extract class structure, methods, annotations, and Selenium API calls
- **API Mapping**: Maps 50+ Selenium Java APIs to their Playwright TypeScript equivalents
- **Code Generator**: Produces clean, idiomatic Playwright test files using `@playwright/test`
- **CLI Interface**: Easy-to-use CLI with `migrate` and `analyze` commands
- **Batch Processing**: Process entire directories of Java test files at once

### Supported Selenium Patterns

| Selenium Java | Playwright TypeScript |
|---|---|
| `driver.get(url)` | `await page.goto(url)` |
| `driver.findElement(By.id("x"))` | `page.locator('#x')` |
| `driver.findElement(By.className("x"))` | `page.locator('.x')` |
| `driver.findElement(By.cssSelector("x"))` | `page.locator('x')` |
| `driver.findElement(By.xpath("x"))` | `page.locator('x')` |
| `driver.findElement(By.name("x"))` | `page.locator('[name="x"]')` |
| `driver.findElement(By.linkText("x"))` | `page.locator('a:has-text("x")')` |
| `.click()` | `.click()` |
| `.sendKeys("text")` | `.fill("text")` |
| `.sendKeys(Keys.ENTER)` | `.press('Enter')` |
| `.getText()` | `.textContent()` |
| `.getAttribute("x")` | `.getAttribute("x")` |
| `.isDisplayed()` | `.isVisible()` |
| `.isEnabled()` | `.isEnabled()` |
| `.isSelected()` | `.isChecked()` |
| `.clear()` | `.clear()` |
| `driver.getTitle()` | `page.title()` |
| `driver.getCurrentUrl()` | `page.url()` |
| `driver.navigate().back()` | `page.goBack()` |
| `driver.navigate().forward()` | `page.goForward()` |
| `driver.navigate().refresh()` | `page.reload()` |
| `new Select(...).selectByVisibleText()` | `.selectOption({ label: ... })` |
| `new Select(...).selectByValue()` | `.selectOption(value)` |
| `Actions.moveToElement()` | `.hover()` |
| `Actions.doubleClick()` | `.dblclick()` |
| `Actions.contextClick()` | `.click({ button: 'right' })` |
| `Actions.dragAndDrop()` | `.dragTo()` |
| `WebDriverWait + ExpectedConditions` | Playwright auto-wait / `expect` assertions |
| `Thread.sleep()` | `page.waitForTimeout()` |
| `Alert.accept/dismiss` | `page.on('dialog', ...)` |
| `switchTo().frame()` | `page.frameLocator()` |
| `Assert.assertEquals()` | `expect().toBe()` |
| `Assert.assertTrue()` | `expect().toBeTruthy()` |
| `Assert.assertFalse()` | `expect().toBeFalsy()` |
| `Assert.assertNotNull()` | `expect().not.toBeNull()` |
| `System.out.println()` | `console.log()` |

### Test Framework Support

- **JUnit 4/5**: `@Before`, `@After`, `@BeforeEach`, `@AfterEach`, `@Test`
- **TestNG**: `@BeforeMethod`, `@AfterMethod`, `@BeforeClass`, `@AfterClass`, `@Test`

## Installation

```bash
cd selenium-to-playwright-migrator
npm install
npm run build
```

## Usage

### Migrate Files

```bash
# Migrate a single file
node dist/index.js migrate -i path/to/SeleniumTest.java -o output/

# Migrate an entire directory
node dist/index.js migrate -i src/test/java/ -o playwright-tests/ --verbose
```

### Analyze Files

```bash
# Analyze test complexity before migration
node dist/index.js analyze -i src/test/java/
```

### Example

Given a Selenium Java test:

```java
@Test
public void testLogin() {
    driver.get("https://example.com/login");
    driver.findElement(By.id("username")).sendKeys("user");
    driver.findElement(By.id("password")).sendKeys("pass");
    driver.findElement(By.cssSelector("button[type='submit']")).click();

    WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(10));
    wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("dashboard")));
    Assert.assertEquals(driver.getTitle(), "Dashboard");
}
```

The migrator produces:

```typescript
import { test, expect } from '@playwright/test';

test.describe('Login', () => {
  test('login', async ({ page }) => {
    await page.goto("https://example.com/login");
    await page.locator('#username').fill("user");
    await page.locator('#password').fill("pass");
    await page.locator("button[type='submit']").click();
    await expect(page.locator('#dashboard')).toBeVisible();
    const title = await page.title();
    expect(title).toBe("Dashboard");
  });
});
```

## Architecture

```
src/
├── index.ts                    # CLI entry point (commander.js)
├── parser/
│   └── javaParser.ts           # Regex-based Java source parser
├── mapper/
│   └── apiMapper.ts            # Selenium → Playwright API mapping
├── generator/
│   └── playwrightGenerator.ts  # TypeScript Playwright code generator
├── types/
│   └── index.ts                # TypeScript type definitions
└── utils/
    └── helpers.ts              # String conversion utilities
```

## Post-Migration Steps

1. Review generated files for `TODO` comments marking patterns that need manual attention
2. Install Playwright: `npm init playwright@latest`
3. Update `playwright.config.ts` with your application URL
4. Run tests: `npx playwright test`
