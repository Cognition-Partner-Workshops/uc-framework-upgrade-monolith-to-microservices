import { PlaywrightAction, ActionType } from '../types';
import {
  extractStringLiteral,
  convertStringConcatenation,
  javaTypeToTypeScript,
  cleanLine,
} from '../utils/helpers';

/**
 * Maps Selenium Java API calls to Playwright TypeScript equivalents.
 * Handles locator strategies, actions, waits, assertions, and browser management.
 */
export class ApiMapper {
  private warnings: string[] = [];
  private unsupportedPatterns: string[] = [];

  getWarnings(): string[] {
    return this.warnings;
  }

  getUnsupportedPatterns(): string[] {
    return this.unsupportedPatterns;
  }

  resetDiagnostics(): void {
    this.warnings = [];
    this.unsupportedPatterns = [];
  }

  /**
   * Map a single line of Selenium Java code to Playwright TypeScript
   */
  mapLine(line: string): PlaywrightAction {
    const trimmed = cleanLine(line);

    if (!trimmed || trimmed === '{' || trimmed === '}') {
      return { type: ActionType.CONTROL_FLOW, code: trimmed, needsAwait: false };
    }

    if (trimmed.startsWith('//') || trimmed.startsWith('/*') || trimmed.startsWith('*')) {
      return { type: ActionType.COMMENT, code: trimmed, needsAwait: false };
    }

    // Try each mapper in order of specificity
    return (
      this.mapDriverCreation(trimmed) ||
      this.mapDriverQuit(trimmed) ||
      this.mapNavigation(trimmed) ||
      this.mapFindElementAction(trimmed) ||
      this.mapFindElements(trimmed) ||
      this.mapVariableElementAction(trimmed) ||
      this.mapExplicitWait(trimmed) ||
      this.mapImplicitWait(trimmed) ||
      this.mapThreadSleep(trimmed) ||
      this.mapAlert(trimmed) ||
      this.mapFrameSwitch(trimmed) ||
      this.mapWindowHandle(trimmed) ||
      this.mapSelect(trimmed) ||
      this.mapActionsChain(trimmed) ||
      this.mapDriverGetters(trimmed) ||
      this.mapAssertions(trimmed) ||
      this.mapJavaContains(trimmed) ||
      this.mapVariableDeclaration(trimmed) ||
      this.mapGenericStatement(trimmed)
    );
  }

  /**
   * Map WebDriver creation
   */
  private mapDriverCreation(line: string): PlaywrightAction | null {
    const patterns = [
      /(?:WebDriver|ChromeDriver|FirefoxDriver|EdgeDriver)\s+(\w+)\s*=\s*new\s+(Chrome|Firefox|Edge|Remote)Driver\s*\(/,
      /(\w+)\s*=\s*new\s+(Chrome|Firefox|Edge|Remote)Driver\s*\(/,
    ];

    for (const pattern of patterns) {
      const match = line.match(pattern);
      if (match) {
        return {
          type: ActionType.BROWSER_MANAGEMENT,
          code: `// Browser is managed by Playwright test fixtures - no manual driver creation needed`,
          needsAwait: false,
          comment: `Migrated from: ${line}`,
        };
      }
    }
    return null;
  }

  /**
   * Map driver.quit() / driver.close()
   */
  private mapDriverQuit(line: string): PlaywrightAction | null {
    if (/\w+\.(quit|close)\(\)/.test(line)) {
      return {
        type: ActionType.BROWSER_MANAGEMENT,
        code: `// Browser cleanup is handled automatically by Playwright test fixtures`,
        needsAwait: false,
        comment: `Migrated from: ${line}`,
      };
    }
    return null;
  }

  /**
   * Map navigation commands
   */
  private mapNavigation(line: string): PlaywrightAction | null {
    // driver.get(url) or driver.navigate().to(url)
    let match = line.match(/\w+\.(?:get|navigate\(\)\.to)\((.+)\)/);
    if (match) {
      const url = this.convertValue(match[1]);
      return {
        type: ActionType.NAVIGATION,
        code: `await page.goto(${url});`,
        needsAwait: true,
      };
    }

    // driver.navigate().back()
    if (/\w+\.navigate\(\)\.back\(\)/.test(line)) {
      return {
        type: ActionType.NAVIGATION,
        code: `await page.goBack();`,
        needsAwait: true,
      };
    }

    // driver.navigate().forward()
    if (/\w+\.navigate\(\)\.forward\(\)/.test(line)) {
      return {
        type: ActionType.NAVIGATION,
        code: `await page.goForward();`,
        needsAwait: true,
      };
    }

    // driver.navigate().refresh()
    if (/\w+\.navigate\(\)\.refresh\(\)/.test(line)) {
      return {
        type: ActionType.NAVIGATION,
        code: `await page.reload();`,
        needsAwait: true,
      };
    }

    return null;
  }

  /**
   * Map findElement + chained action
   */
  private mapFindElementAction(line: string): PlaywrightAction | null {
    // Pattern: driver.findElement(By.xxx("value")).action()
    const findElementPattern =
      /(\w+\s*=\s*)?(?:\w+\.)?findElement\(By\.(\w+)\((.+?)\)\)(?:\.(.+))?/;
    const match = line.match(findElementPattern);
    if (!match) return null;

    const assignment = match[1]?.trim();
    const locatorStrategy = match[2];
    const locatorValue = match[3];
    const action = match[4];

    const playwrightLocator = this.mapLocatorStrategy(
      locatorStrategy,
      locatorValue
    );

    if (!action) {
      // Just a findElement with possible assignment
      if (assignment) {
        const varName = assignment.replace(/\s*=\s*$/, '').replace(/^(?:WebElement|final)\s+/, '');
        return {
          type: ActionType.LOCATOR,
          code: `const ${varName} = page.locator(${playwrightLocator});`,
          needsAwait: false,
        };
      }
      return {
        type: ActionType.LOCATOR,
        code: `page.locator(${playwrightLocator});`,
        needsAwait: false,
      };
    }

    return this.mapElementAction(playwrightLocator, action, assignment);
  }

  /**
   * Map findElements (plural)
   */
  private mapFindElements(line: string): PlaywrightAction | null {
    const match = line.match(
      /(\w+\s*=\s*)?(?:\w+\.)?findElements\(By\.(\w+)\((.+?)\)\)/
    );
    if (!match) return null;

    const assignment = match[1]?.trim();
    const locatorStrategy = match[2];
    const locatorValue = match[3];
    const playwrightLocator = this.mapLocatorStrategy(locatorStrategy, locatorValue);

    if (assignment) {
      const varName = assignment.replace(/\s*=\s*$/, '').replace(/^(?:List<WebElement>|final)\s+/, '');
      return {
        type: ActionType.LOCATOR,
        code: `const ${varName} = page.locator(${playwrightLocator});`,
        needsAwait: false,
        comment: 'Use .all() to get array of locators, or .count() for count',
      };
    }

    return {
      type: ActionType.LOCATOR,
      code: `page.locator(${playwrightLocator});`,
      needsAwait: false,
    };
  }

  /**
   * Map an element action (click, sendKeys, getText, etc.)
   */
  private mapElementAction(
    locator: string,
    action: string,
    assignment?: string
  ): PlaywrightAction {
    // click()
    if (action === 'click()') {
      return {
        type: ActionType.ACTION,
        code: `await page.locator(${locator}).click();`,
        needsAwait: true,
      };
    }

    // sendKeys("text")
    const sendKeysMatch = action.match(/sendKeys\((.+)\)/);
    if (sendKeysMatch) {
      const value = this.convertValue(sendKeysMatch[1]);
      // Check if it's a Keys constant
      if (sendKeysMatch[1].includes('Keys.')) {
        const keyMapping = this.mapSeleniumKey(sendKeysMatch[1]);
        return {
          type: ActionType.ACTION,
          code: `await page.locator(${locator}).press(${keyMapping});`,
          needsAwait: true,
        };
      }
      return {
        type: ActionType.ACTION,
        code: `await page.locator(${locator}).fill(${value});`,
        needsAwait: true,
      };
    }

    // getText()
    if (action === 'getText()') {
      if (assignment) {
        const varName = assignment.replace(/\s*=\s*$/, '').replace(/^(?:String|final)\s+/, '');
        return {
          type: ActionType.ACTION,
          code: `const ${varName} = await page.locator(${locator}).textContent();`,
          needsAwait: true,
        };
      }
      return {
        type: ActionType.ACTION,
        code: `await page.locator(${locator}).textContent();`,
        needsAwait: true,
      };
    }

    // getAttribute("attr")
    const getAttrMatch = action.match(/getAttribute\((.+)\)/);
    if (getAttrMatch) {
      const attr = this.convertValue(getAttrMatch[1]);
      if (assignment) {
        const varName = assignment.replace(/\s*=\s*$/, '').replace(/^(?:String|final)\s+/, '');
        return {
          type: ActionType.ACTION,
          code: `const ${varName} = await page.locator(${locator}).getAttribute(${attr});`,
          needsAwait: true,
        };
      }
      return {
        type: ActionType.ACTION,
        code: `await page.locator(${locator}).getAttribute(${attr});`,
        needsAwait: true,
      };
    }

    // isDisplayed()
    if (action === 'isDisplayed()') {
      if (assignment) {
        const varName = assignment.replace(/\s*=\s*$/, '').replace(/^(?:boolean|Boolean|final)\s+/, '');
        return {
          type: ActionType.ACTION,
          code: `const ${varName} = await page.locator(${locator}).isVisible();`,
          needsAwait: true,
        };
      }
      return {
        type: ActionType.ACTION,
        code: `await page.locator(${locator}).isVisible();`,
        needsAwait: true,
      };
    }

    // isEnabled()
    if (action === 'isEnabled()') {
      if (assignment) {
        const varName = assignment.replace(/\s*=\s*$/, '').replace(/^(?:boolean|Boolean|final)\s+/, '');
        return {
          type: ActionType.ACTION,
          code: `const ${varName} = await page.locator(${locator}).isEnabled();`,
          needsAwait: true,
        };
      }
      return {
        type: ActionType.ACTION,
        code: `await page.locator(${locator}).isEnabled();`,
        needsAwait: true,
      };
    }

    // isSelected()
    if (action === 'isSelected()') {
      if (assignment) {
        const varName = assignment.replace(/\s*=\s*$/, '').replace(/^(?:boolean|Boolean|final)\s+/, '');
        return {
          type: ActionType.ACTION,
          code: `const ${varName} = await page.locator(${locator}).isChecked();`,
          needsAwait: true,
        };
      }
      return {
        type: ActionType.ACTION,
        code: `await page.locator(${locator}).isChecked();`,
        needsAwait: true,
      };
    }

    // clear()
    if (action === 'clear()') {
      return {
        type: ActionType.ACTION,
        code: `await page.locator(${locator}).clear();`,
        needsAwait: true,
      };
    }

    // submit()
    if (action === 'submit()') {
      this.warnings.push(
        'Playwright does not have a direct submit(). Using click on submit button or pressing Enter instead.'
      );
      return {
        type: ActionType.ACTION,
        code: `await page.locator(${locator}).press('Enter'); // Migrated from submit() - verify this is correct`,
        needsAwait: true,
      };
    }

    // getCssValue("prop")
    const cssMatch = action.match(/getCssValue\((.+)\)/);
    if (cssMatch) {
      const prop = this.convertValue(cssMatch[1]);
      if (assignment) {
        const varName = assignment.replace(/\s*=\s*$/, '').replace(/^(?:String|final)\s+/, '');
        return {
          type: ActionType.ACTION,
          code: `const ${varName} = await page.locator(${locator}).evaluate(el => getComputedStyle(el).getPropertyValue(${prop}));`,
          needsAwait: true,
        };
      }
      return {
        type: ActionType.ACTION,
        code: `await page.locator(${locator}).evaluate(el => getComputedStyle(el).getPropertyValue(${prop}));`,
        needsAwait: true,
      };
    }

    // Fallback for unknown actions
    this.unsupportedPatterns.push(`Unknown element action: .${action}`);
    return {
      type: ActionType.UNKNOWN,
      code: `// TODO: Manually migrate - page.locator(${locator}).${action}`,
      needsAwait: false,
      comment: `Unsupported action: ${action}`,
    };
  }

  /**
   * Map Selenium locator strategy to Playwright locator
   */
  private mapLocatorStrategy(strategy: string, value: string): string {
    const cleanValue = value.trim();

    switch (strategy) {
      case 'id':
        return `'#${extractStringLiteral(cleanValue)}'`;
      case 'className':
        return `'.${extractStringLiteral(cleanValue)}'`;
      case 'cssSelector':
      case 'css':
        return cleanValue;
      case 'xpath':
        return cleanValue;
      case 'name':
        return `'[name="${extractStringLiteral(cleanValue)}"]'`;
      case 'tagName':
        return cleanValue;
      case 'linkText':
        return `'a:has-text("${extractStringLiteral(cleanValue)}")'`;
      case 'partialLinkText':
        return `'a:has-text("${extractStringLiteral(cleanValue)}")'`;
      default:
        this.warnings.push(`Unknown locator strategy: ${strategy}`);
        return cleanValue;
    }
  }

  /**
   * Map Selenium Keys constants to Playwright key names
   */
  private mapSeleniumKey(keysExpr: string): string {
    const keyMap: Record<string, string> = {
      'Keys.ENTER': "'Enter'",
      'Keys.RETURN': "'Enter'",
      'Keys.TAB': "'Tab'",
      'Keys.ESCAPE': "'Escape'",
      'Keys.BACK_SPACE': "'Backspace'",
      'Keys.DELETE': "'Delete'",
      'Keys.SPACE': "' '",
      'Keys.ARROW_UP': "'ArrowUp'",
      'Keys.ARROW_DOWN': "'ArrowDown'",
      'Keys.ARROW_LEFT': "'ArrowLeft'",
      'Keys.ARROW_RIGHT': "'ArrowRight'",
      'Keys.HOME': "'Home'",
      'Keys.END': "'End'",
      'Keys.PAGE_UP': "'PageUp'",
      'Keys.PAGE_DOWN': "'PageDown'",
      'Keys.F1': "'F1'",
      'Keys.F2': "'F2'",
      'Keys.F3': "'F3'",
      'Keys.F4': "'F4'",
      'Keys.F5': "'F5'",
      'Keys.CONTROL': "'Control'",
      'Keys.SHIFT': "'Shift'",
      'Keys.ALT': "'Alt'",
    };

    // Handle Keys.chord(Keys.CONTROL, "a") pattern
    const chordMatch = keysExpr.match(/Keys\.chord\((.+)\)/);
    if (chordMatch) {
      const parts = chordMatch[1].split(',').map((p) => p.trim());
      const mapped = parts.map((p) => {
        if (p.startsWith('Keys.')) {
          return keyMap[p]?.replace(/'/g, '') || p;
        }
        return extractStringLiteral(p);
      });
      return `'${mapped.join('+')}'`;
    }

    return keyMap[keysExpr.trim()] || `'${keysExpr}'`;
  }

  /**
   * Map WebDriverWait / explicit waits
   */
  private mapExplicitWait(line: string): PlaywrightAction | null {
    // WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(10))
    if (/new\s+WebDriverWait/.test(line)) {
      return {
        type: ActionType.WAIT,
        code: `// Playwright has built-in auto-waiting - explicit waits are typically not needed`,
        needsAwait: false,
        comment: `Migrated from: ${line}`,
      };
    }

    // wait.until(ExpectedConditions.visibilityOfElementLocated(By.xxx("value")))
    const waitUntilMatch = line.match(
      /\w+\.until\(ExpectedConditions\.(\w+)\(By\.(\w+)\((.+?)\)\)\)/
    );
    if (waitUntilMatch) {
      const condition = waitUntilMatch[1];
      const strategy = waitUntilMatch[2];
      const value = waitUntilMatch[3];
      const locator = this.mapLocatorStrategy(strategy, value);

      return this.mapExpectedCondition(condition, locator, line);
    }

    // wait.until(ExpectedConditions.titleContains("text"))
    const titleWaitMatch = line.match(
      /\w+\.until\(ExpectedConditions\.(titleContains|titleIs)\((.+?)\)\)/
    );
    if (titleWaitMatch) {
      const value = this.convertValue(titleWaitMatch[2]);
      if (titleWaitMatch[1] === 'titleIs') {
        return {
          type: ActionType.WAIT,
          code: `await expect(page).toHaveTitle(${value});`,
          needsAwait: true,
        };
      }
      return {
        type: ActionType.WAIT,
        code: `await expect(page).toHaveTitle(new RegExp(${value}));`,
        needsAwait: true,
      };
    }

    // wait.until(ExpectedConditions.urlContains("text"))
    const urlWaitMatch = line.match(
      /\w+\.until\(ExpectedConditions\.(urlContains|urlToBe)\((.+?)\)\)/
    );
    if (urlWaitMatch) {
      const value = this.convertValue(urlWaitMatch[2]);
      if (urlWaitMatch[1] === 'urlToBe') {
        return {
          type: ActionType.WAIT,
          code: `await expect(page).toHaveURL(${value});`,
          needsAwait: true,
        };
      }
      return {
        type: ActionType.WAIT,
        code: `await expect(page).toHaveURL(new RegExp(${value}));`,
        needsAwait: true,
      };
    }

    return null;
  }

  /**
   * Map ExpectedConditions to Playwright assertions/waits
   */
  private mapExpectedCondition(
    condition: string,
    locator: string,
    originalLine: string
  ): PlaywrightAction {
    switch (condition) {
      case 'visibilityOfElementLocated':
      case 'visibilityOf':
        return {
          type: ActionType.WAIT,
          code: `await expect(page.locator(${locator})).toBeVisible();`,
          needsAwait: true,
        };
      case 'invisibilityOfElementLocated':
        return {
          type: ActionType.WAIT,
          code: `await expect(page.locator(${locator})).toBeHidden();`,
          needsAwait: true,
        };
      case 'elementToBeClickable':
        return {
          type: ActionType.WAIT,
          code: `await expect(page.locator(${locator})).toBeEnabled();`,
          needsAwait: true,
        };
      case 'presenceOfElementLocated':
        return {
          type: ActionType.WAIT,
          code: `await page.locator(${locator}).waitFor({ state: 'attached' });`,
          needsAwait: true,
        };
      case 'stalenessOf':
        return {
          type: ActionType.WAIT,
          code: `await page.locator(${locator}).waitFor({ state: 'detached' });`,
          needsAwait: true,
        };
      case 'textToBePresentInElementLocated':
        return {
          type: ActionType.WAIT,
          code: `await expect(page.locator(${locator})).toContainText(/* expected text */);`,
          needsAwait: true,
          comment: 'TODO: Add expected text value',
        };
      default:
        this.warnings.push(
          `Unsupported ExpectedCondition: ${condition}`
        );
        return {
          type: ActionType.UNKNOWN,
          code: `// TODO: Manually migrate ExpectedCondition.${condition} for locator ${locator}`,
          needsAwait: false,
          comment: `Original: ${originalLine}`,
        };
    }
  }

  /**
   * Map implicit waits
   */
  private mapImplicitWait(line: string): PlaywrightAction | null {
    if (/\.manage\(\)\.timeouts\(\)\.implicitlyWait\(/.test(line)) {
      return {
        type: ActionType.WAIT,
        code: `// Playwright uses auto-waiting by default - implicit waits are not needed`,
        needsAwait: false,
        comment: `Migrated from: ${line}`,
      };
    }
    return null;
  }

  /**
   * Map Thread.sleep()
   */
  private mapThreadSleep(line: string): PlaywrightAction | null {
    const match = line.match(/Thread\.sleep\((\d+)\)/);
    if (match) {
      this.warnings.push(
        'Thread.sleep() converted to page.waitForTimeout(). Consider using proper Playwright waits instead.'
      );
      return {
        type: ActionType.WAIT,
        code: `await page.waitForTimeout(${match[1]}); // Consider replacing with proper wait`,
        needsAwait: true,
      };
    }
    return null;
  }

  /**
   * Map alert/dialog handling
   */
  private mapAlert(line: string): PlaywrightAction | null {
    // driver.switchTo().alert()
    if (/\w+\.switchTo\(\)\.alert\(\)/.test(line)) {
      if (line.includes('.accept()')) {
        return {
          type: ActionType.ACTION,
          code: `page.on('dialog', dialog => dialog.accept());`,
          needsAwait: false,
          comment: 'Set up dialog handler before triggering the dialog',
        };
      }
      if (line.includes('.dismiss()')) {
        return {
          type: ActionType.ACTION,
          code: `page.on('dialog', dialog => dialog.dismiss());`,
          needsAwait: false,
        };
      }
      if (line.includes('.getText()')) {
        return {
          type: ActionType.ACTION,
          code: `page.on('dialog', dialog => { const alertText = dialog.message(); dialog.accept(); });`,
          needsAwait: false,
        };
      }
      const sendKeysMatch = line.match(/\.sendKeys\((.+)\)/);
      if (sendKeysMatch) {
        const value = this.convertValue(sendKeysMatch[1]);
        return {
          type: ActionType.ACTION,
          code: `page.on('dialog', dialog => dialog.accept(${value}));`,
          needsAwait: false,
        };
      }

      return {
        type: ActionType.ACTION,
        code: `// TODO: Handle alert - page.on('dialog', dialog => dialog.accept());`,
        needsAwait: false,
      };
    }
    return null;
  }

  /**
   * Map frame switching
   */
  private mapFrameSwitch(line: string): PlaywrightAction | null {
    // driver.switchTo().frame("name")
    const frameMatch = line.match(/\w+\.switchTo\(\)\.frame\((.+)\)/);
    if (frameMatch) {
      const frameRef = this.convertValue(frameMatch[1]);
      return {
        type: ActionType.ACTION,
        code: `const frame = page.frameLocator(${frameRef});`,
        needsAwait: false,
        comment:
          'Use frame.locator() for elements within the frame',
      };
    }

    // driver.switchTo().defaultContent()
    if (/\w+\.switchTo\(\)\.defaultContent\(\)/.test(line)) {
      return {
        type: ActionType.ACTION,
        code: `// Switched back to main frame - use page.locator() directly`,
        needsAwait: false,
      };
    }

    // driver.switchTo().parentFrame()
    if (/\w+\.switchTo\(\)\.parentFrame\(\)/.test(line)) {
      return {
        type: ActionType.ACTION,
        code: `// Switched to parent frame - use parent frame locator`,
        needsAwait: false,
      };
    }

    return null;
  }

  /**
   * Map window handle operations
   */
  private mapWindowHandle(line: string): PlaywrightAction | null {
    // driver.getWindowHandle()
    if (/\w+\.getWindowHandle\(\)/.test(line)) {
      return {
        type: ActionType.BROWSER_MANAGEMENT,
        code: `// Window handles are managed differently in Playwright - use page/context`,
        needsAwait: false,
      };
    }

    // driver.getWindowHandles()
    if (/\w+\.getWindowHandles\(\)/.test(line)) {
      return {
        type: ActionType.BROWSER_MANAGEMENT,
        code: `const pages = context.pages();`,
        needsAwait: false,
      };
    }

    // driver.switchTo().window(handle)
    const windowMatch = line.match(/\w+\.switchTo\(\)\.window\((.+)\)/);
    if (windowMatch) {
      return {
        type: ActionType.BROWSER_MANAGEMENT,
        code: `// Use context.pages() to get all pages and switch between them`,
        needsAwait: false,
        comment: 'Playwright uses separate Page objects instead of window handles',
      };
    }

    // driver.manage().window().maximize()
    if (/\.manage\(\)\.window\(\)\.maximize\(\)/.test(line)) {
      return {
        type: ActionType.BROWSER_MANAGEMENT,
        code: `// Window size can be set in playwright.config.ts via use.viewport`,
        needsAwait: false,
      };
    }

    // driver.manage().window().setSize()
    const sizeMatch = line.match(
      /\.manage\(\)\.window\(\)\.setSize\(new\s+Dimension\((\d+),\s*(\d+)\)\)/
    );
    if (sizeMatch) {
      return {
        type: ActionType.BROWSER_MANAGEMENT,
        code: `await page.setViewportSize({ width: ${sizeMatch[1]}, height: ${sizeMatch[2]} });`,
        needsAwait: true,
      };
    }

    return null;
  }

  /**
   * Map Select (dropdown) operations
   */
  private mapSelect(line: string): PlaywrightAction | null {
    // new Select(element).selectByVisibleText("text")
    const selectByTextMatch = line.match(
      /new\s+Select\((?:\w+\.)?findElement\(By\.(\w+)\((.+?)\)\)\)\.selectByVisibleText\((.+)\)/
    );
    if (selectByTextMatch) {
      const locator = this.mapLocatorStrategy(
        selectByTextMatch[1],
        selectByTextMatch[2]
      );
      const text = this.convertValue(selectByTextMatch[3]);
      return {
        type: ActionType.ACTION,
        code: `await page.locator(${locator}).selectOption({ label: ${text} });`,
        needsAwait: true,
      };
    }

    // new Select(element).selectByValue("value")
    const selectByValueMatch = line.match(
      /new\s+Select\((?:\w+\.)?findElement\(By\.(\w+)\((.+?)\)\)\)\.selectByValue\((.+)\)/
    );
    if (selectByValueMatch) {
      const locator = this.mapLocatorStrategy(
        selectByValueMatch[1],
        selectByValueMatch[2]
      );
      const value = this.convertValue(selectByValueMatch[3]);
      return {
        type: ActionType.ACTION,
        code: `await page.locator(${locator}).selectOption(${value});`,
        needsAwait: true,
      };
    }

    // new Select(element).selectByIndex(index)
    const selectByIndexMatch = line.match(
      /new\s+Select\((?:\w+\.)?findElement\(By\.(\w+)\((.+?)\)\)\)\.selectByIndex\((.+)\)/
    );
    if (selectByIndexMatch) {
      const locator = this.mapLocatorStrategy(
        selectByIndexMatch[1],
        selectByIndexMatch[2]
      );
      return {
        type: ActionType.ACTION,
        code: `await page.locator(${locator}).selectOption({ index: ${selectByIndexMatch[3]} });`,
        needsAwait: true,
      };
    }

    return null;
  }

  /**
   * Map variable-based element actions (e.g., element.click(), element.sendKeys(), element.clear())
   * This handles cases where a WebElement is stored in a variable first
   */
  private mapVariableElementAction(line: string): PlaywrightAction | null {
    // Match: variable.action() pattern (not driver.xxx or new Xxx)
    const varActionMatch = line.match(/^(\w+)\.(click|sendKeys|clear|getText|getAttribute|isDisplayed|isEnabled|isSelected|submit|getCssValue)\((.*)\)$/);
    if (!varActionMatch) return null;

    const varName = varActionMatch[1];
    // Skip if it looks like a driver/wait/actions call
    if (['driver', 'wait', 'actions', 'select', 'js', 'executor'].includes(varName)) return null;

    const action = varActionMatch[2];
    const args = varActionMatch[3];

    switch (action) {
      case 'click':
        return {
          type: ActionType.ACTION,
          code: `await ${varName}.click();`,
          needsAwait: true,
        };
      case 'sendKeys': {
        const value = this.convertValue(args);
        if (args.includes('Keys.')) {
          const keyMapping = this.mapSeleniumKey(args);
          return {
            type: ActionType.ACTION,
            code: `await ${varName}.press(${keyMapping});`,
            needsAwait: true,
          };
        }
        return {
          type: ActionType.ACTION,
          code: `await ${varName}.fill(${value});`,
          needsAwait: true,
        };
      }
      case 'clear':
        return {
          type: ActionType.ACTION,
          code: `await ${varName}.clear();`,
          needsAwait: true,
        };
      case 'getText': {
        return {
          type: ActionType.ACTION,
          code: `await ${varName}.textContent()`,
          needsAwait: true,
        };
      }
      case 'getAttribute': {
        const attr = this.convertValue(args);
        return {
          type: ActionType.ACTION,
          code: `await ${varName}.getAttribute(${attr})`,
          needsAwait: true,
        };
      }
      case 'isDisplayed':
        return {
          type: ActionType.ACTION,
          code: `await ${varName}.isVisible()`,
          needsAwait: true,
        };
      case 'isEnabled':
        return {
          type: ActionType.ACTION,
          code: `await ${varName}.isEnabled()`,
          needsAwait: true,
        };
      case 'isSelected':
        return {
          type: ActionType.ACTION,
          code: `await ${varName}.isChecked()`,
          needsAwait: true,
        };
      case 'submit':
        this.warnings.push(
          'Playwright does not have a direct submit(). Using press Enter instead.'
        );
        return {
          type: ActionType.ACTION,
          code: `await ${varName}.press('Enter'); // Migrated from submit()`,
          needsAwait: true,
        };
      case 'getCssValue': {
        const prop = this.convertValue(args);
        return {
          type: ActionType.ACTION,
          code: `await ${varName}.evaluate(el => getComputedStyle(el).getPropertyValue(${prop}))`,
          needsAwait: true,
        };
      }
      default:
        return null;
    }
  }

  /**
   * Map .contains() calls to .includes() for TypeScript
   */
  private mapJavaContains(line: string): PlaywrightAction | null {
    // Match assertions that use .contains() - e.g., assertTrue(url.contains("/dashboard"))
    // This is handled by assertion mappers, so just convert standalone .contains calls
    const containsMatch = line.match(/^(\w+)\.contains\((.+)\)$/);
    if (containsMatch) {
      return {
        type: ActionType.ACTION,
        code: `${containsMatch[1]}.includes(${this.convertValue(containsMatch[2])})`,
        needsAwait: false,
      };
    }
    return null;
  }

  /**
   * Map Actions class (hover, drag-drop, double-click, etc.)
   */
  private mapActionsChain(line: string): PlaywrightAction | null {
    // new Actions(driver).moveToElement(element).perform()
    if (/Actions\(/.test(line) && line.includes('moveToElement')) {
      const elementMatch = line.match(
        /moveToElement\((?:\w+\.)?findElement\(By\.(\w+)\((.+?)\)\)\)/
      );
      if (elementMatch) {
        const locator = this.mapLocatorStrategy(
          elementMatch[1],
          elementMatch[2]
        );
        return {
          type: ActionType.ACTION,
          code: `await page.locator(${locator}).hover();`,
          needsAwait: true,
        };
      }
      // Handle variable-based element: actions.moveToElement(varName).perform()
      const varElementMatch = line.match(/moveToElement\((\w+)\)/);
      if (varElementMatch) {
        return {
          type: ActionType.ACTION,
          code: `await ${varElementMatch[1]}.hover();`,
          needsAwait: true,
        };
      }
    }

    // Handle variable-based actions: actions.moveToElement(varName).perform()
    const varActionsMatch = line.match(/(\w+)\.moveToElement\((\w+)\)\.perform\(\)/);
    if (varActionsMatch) {
      return {
        type: ActionType.ACTION,
        code: `await ${varActionsMatch[2]}.hover();`,
        needsAwait: true,
      };
    }

    // Actions doubleClick
    if (/Actions\(/.test(line) && line.includes('doubleClick')) {
      const elementMatch = line.match(
        /doubleClick\((?:\w+\.)?findElement\(By\.(\w+)\((.+?)\)\)\)/
      );
      if (elementMatch) {
        const locator = this.mapLocatorStrategy(
          elementMatch[1],
          elementMatch[2]
        );
        return {
          type: ActionType.ACTION,
          code: `await page.locator(${locator}).dblclick();`,
          needsAwait: true,
        };
      }
    }

    // Actions contextClick (right-click)
    if (/Actions\(/.test(line) && line.includes('contextClick')) {
      const elementMatch = line.match(
        /contextClick\((?:\w+\.)?findElement\(By\.(\w+)\((.+?)\)\)\)/
      );
      if (elementMatch) {
        const locator = this.mapLocatorStrategy(
          elementMatch[1],
          elementMatch[2]
        );
        return {
          type: ActionType.ACTION,
          code: `await page.locator(${locator}).click({ button: 'right' });`,
          needsAwait: true,
        };
      }
    }

    // Actions dragAndDrop
    if (/Actions\(/.test(line) && line.includes('dragAndDrop')) {
      const dragMatch = line.match(
        /dragAndDrop\((?:\w+\.)?findElement\(By\.(\w+)\((.+?)\)\)\s*,\s*(?:\w+\.)?findElement\(By\.(\w+)\((.+?)\)\)\)/
      );
      if (dragMatch) {
        const sourceLocator = this.mapLocatorStrategy(dragMatch[1], dragMatch[2]);
        const targetLocator = this.mapLocatorStrategy(dragMatch[3], dragMatch[4]);
        return {
          type: ActionType.ACTION,
          code: `await page.locator(${sourceLocator}).dragTo(page.locator(${targetLocator}));`,
          needsAwait: true,
        };
      }
    }

    return null;
  }

  /**
   * Map driver getter methods
   */
  private mapDriverGetters(line: string): PlaywrightAction | null {
    // driver.getTitle()
    if (/\w+\.getTitle\(\)/.test(line)) {
      const assignMatch = line.match(/(\w+)\s*=\s*\w+\.getTitle\(\)/);
      if (assignMatch) {
        return {
          type: ActionType.ACTION,
          code: `const ${assignMatch[1]} = await page.title();`,
          needsAwait: true,
        };
      }
      return {
        type: ActionType.ACTION,
        code: `await page.title();`,
        needsAwait: true,
      };
    }

    // driver.getCurrentUrl()
    if (/\w+\.getCurrentUrl\(\)/.test(line)) {
      const assignMatch = line.match(/(\w+)\s*=\s*\w+\.getCurrentUrl\(\)/);
      if (assignMatch) {
        return {
          type: ActionType.ACTION,
          code: `const ${assignMatch[1]} = page.url();`,
          needsAwait: false,
        };
      }
      return {
        type: ActionType.ACTION,
        code: `page.url();`,
        needsAwait: false,
      };
    }

    // driver.getPageSource()
    if (/\w+\.getPageSource\(\)/.test(line)) {
      const assignMatch = line.match(/(\w+)\s*=\s*\w+\.getPageSource\(\)/);
      if (assignMatch) {
        return {
          type: ActionType.ACTION,
          code: `const ${assignMatch[1]} = await page.content();`,
          needsAwait: true,
        };
      }
      return {
        type: ActionType.ACTION,
        code: `await page.content();`,
        needsAwait: true,
      };
    }

    // driver.manage().getCookies()
    if (/\.manage\(\)\.getCookies\(\)/.test(line)) {
      return {
        type: ActionType.ACTION,
        code: `const cookies = await context.cookies();`,
        needsAwait: true,
      };
    }

    // driver.manage().addCookie()
    const cookieMatch = line.match(/\.manage\(\)\.addCookie\((.+)\)/);
    if (cookieMatch) {
      return {
        type: ActionType.ACTION,
        code: `await context.addCookies([/* cookie object */]); // TODO: Convert cookie parameters`,
        needsAwait: true,
      };
    }

    // driver.manage().deleteAllCookies()
    if (/\.manage\(\)\.deleteAllCookies\(\)/.test(line)) {
      return {
        type: ActionType.ACTION,
        code: `await context.clearCookies();`,
        needsAwait: true,
      };
    }

    return null;
  }

  /**
   * Map assertion statements (TestNG/JUnit Assert)
   */
  private mapAssertions(line: string): PlaywrightAction | null {
    // Assert.assertEquals(actual, expected)
    const assertEqualsMatch = line.match(
      /Assert\.assertEquals\((.+?),\s*(.+)\)/
    );
    if (assertEqualsMatch) {
      const actual = this.convertValue(assertEqualsMatch[1]);
      const expected = this.convertValue(assertEqualsMatch[2]);
      return {
        type: ActionType.ASSERTION,
        code: `expect(${actual}).toBe(${expected});`,
        needsAwait: false,
      };
    }

    // Assert.assertTrue(condition)
    const assertTrueMatch = line.match(/Assert\.assertTrue\((.+)\)/);
    if (assertTrueMatch) {
      const condition = this.convertValue(assertTrueMatch[1]);
      return {
        type: ActionType.ASSERTION,
        code: `expect(${condition}).toBeTruthy();`,
        needsAwait: false,
      };
    }

    // Assert.assertFalse(condition)
    const assertFalseMatch = line.match(/Assert\.assertFalse\((.+)\)/);
    if (assertFalseMatch) {
      const condition = this.convertValue(assertFalseMatch[1]);
      return {
        type: ActionType.ASSERTION,
        code: `expect(${condition}).toBeFalsy();`,
        needsAwait: false,
      };
    }

    // Assert.assertNotNull(value)
    const assertNotNullMatch = line.match(/Assert\.assertNotNull\((.+)\)/);
    if (assertNotNullMatch) {
      const value = this.convertValue(assertNotNullMatch[1]);
      return {
        type: ActionType.ASSERTION,
        code: `expect(${value}).not.toBeNull();`,
        needsAwait: false,
      };
    }

    // Assert.assertNull(value)
    const assertNullMatch = line.match(/Assert\.assertNull\((.+)\)/);
    if (assertNullMatch) {
      const value = this.convertValue(assertNullMatch[1]);
      return {
        type: ActionType.ASSERTION,
        code: `expect(${value}).toBeNull();`,
        needsAwait: false,
      };
    }

    // assertTrue / assertFalse without Assert. prefix (JUnit static imports)
    const staticAssertTrueMatch = line.match(/^assertTrue\((.+)\)/);
    if (staticAssertTrueMatch) {
      return {
        type: ActionType.ASSERTION,
        code: `expect(${this.convertValue(staticAssertTrueMatch[1])}).toBeTruthy();`,
        needsAwait: false,
      };
    }

    const staticAssertFalseMatch = line.match(/^assertFalse\((.+)\)/);
    if (staticAssertFalseMatch) {
      return {
        type: ActionType.ASSERTION,
        code: `expect(${this.convertValue(staticAssertFalseMatch[1])}).toBeFalsy();`,
        needsAwait: false,
      };
    }

    // assertEquals without Assert. prefix
    const staticAssertEqualsMatch = line.match(
      /^assertEquals\((.+?),\s*(.+)\)/
    );
    if (staticAssertEqualsMatch) {
      return {
        type: ActionType.ASSERTION,
        code: `expect(${this.convertValue(staticAssertEqualsMatch[1])}).toBe(${this.convertValue(staticAssertEqualsMatch[2])});`,
        needsAwait: false,
      };
    }

    // assertNotNull without Assert. prefix
    const staticAssertNotNullMatch = line.match(/^assertNotNull\((.+)\)/);
    if (staticAssertNotNullMatch) {
      return {
        type: ActionType.ASSERTION,
        code: `expect(${this.convertValue(staticAssertNotNullMatch[1])}).not.toBeNull();`,
        needsAwait: false,
      };
    }

    // assertNull without Assert. prefix
    const staticAssertNullMatch = line.match(/^assertNull\((.+)\)/);
    if (staticAssertNullMatch) {
      return {
        type: ActionType.ASSERTION,
        code: `expect(${this.convertValue(staticAssertNullMatch[1])}).toBeNull();`,
        needsAwait: false,
      };
    }

    return null;
  }

  /**
   * Map variable declarations with Java types to TypeScript
   */
  private mapVariableDeclaration(line: string): PlaywrightAction | null {
    // Variable declaration with type: String x = "value";
    const varDeclMatch = line.match(
      /^(?:final\s+)?(\w+(?:<[\w,\s<>]+>)?)\s+(\w+)\s*=\s*(.+)/
    );
    if (
      varDeclMatch &&
      !line.includes('findElement') &&
      !line.includes('new WebDriverWait') &&
      !line.includes('new ChromeDriver') &&
      !line.includes('new FirefoxDriver') &&
      !line.includes('new EdgeDriver')
    ) {
      const javaType = varDeclMatch[1];
      const varName = varDeclMatch[2];
      const value = this.convertValue(varDeclMatch[3]);

      // Skip if it's a test framework type
      if (
        ['WebDriver', 'ChromeDriver', 'FirefoxDriver', 'EdgeDriver', 'RemoteWebDriver'].includes(
          javaType
        )
      ) {
        return null;
      }

      const tsType = javaTypeToTypeScript(javaType);
      if (tsType !== javaType) {
        return {
          type: ActionType.VARIABLE_DECLARATION,
          code: `const ${varName}: ${tsType} = ${value};`,
          needsAwait: false,
        };
      }
      return {
        type: ActionType.VARIABLE_DECLARATION,
        code: `const ${varName} = ${value};`,
        needsAwait: false,
      };
    }
    return null;
  }

  /**
   * Map generic/unrecognized statements
   */
  private mapGenericStatement(line: string): PlaywrightAction {
    // System.out.println -> console.log
    const printMatch = line.match(/System\.out\.println\((.+)\)/);
    if (printMatch) {
      return {
        type: ActionType.ACTION,
        code: `console.log(${this.convertValue(printMatch[1])});`,
        needsAwait: false,
      };
    }

    // If we can't map it, leave a TODO comment
    this.unsupportedPatterns.push(line);
    return {
      type: ActionType.UNKNOWN,
      code: `// TODO: Manually migrate - ${line}`,
      needsAwait: false,
    };
  }

  /**
   * Convert a Java value expression to TypeScript
   */
  private convertValue(value: string): string {
    let result = value.trim();

    // Handle string concatenation
    if (result.includes('+') && result.includes('"')) {
      result = convertStringConcatenation(result);
    }

    // Convert Selenium element methods to Playwright equivalents within expressions
    result = result.replace(/\.isDisplayed\(\)/g, '.isVisible()');
    result = result.replace(/\.isSelected\(\)/g, '.isChecked()');
    result = result.replace(/\.getText\(\)/g, '.textContent()');
    result = result.replace(/\.contains\(/g, '.includes(');

    return result;
  }
}
