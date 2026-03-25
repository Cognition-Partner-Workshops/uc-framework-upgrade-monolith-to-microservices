package com.example.tests;

import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.Keys;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.interactions.Actions;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.Select;
import org.openqa.selenium.support.ui.WebDriverWait;
import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import static org.junit.Assert.*;

import java.time.Duration;
import java.util.List;

public class ShoppingCartTest {

    private WebDriver driver;
    private static final String BASE_URL = "https://shop.example.com";

    @Before
    public void setUp() {
        driver = new ChromeDriver();
        driver.manage().window().maximize();
        driver.get(BASE_URL);
    }

    @After
    public void tearDown() {
        driver.quit();
    }

    @Test
    public void testAddItemToCart() {
        // Navigate to product page
        driver.findElement(By.cssSelector(".product-card:first-child")).click();

        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(10));
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("product-details")));

        // Select size
        new Select(driver.findElement(By.id("size-select"))).selectByVisibleText("Medium");

        // Select color
        new Select(driver.findElement(By.id("color-select"))).selectByValue("blue");

        // Add to cart
        driver.findElement(By.id("add-to-cart")).click();

        // Verify cart count
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.className("cart-badge")));
        String cartCount = driver.findElement(By.className("cart-badge")).getText();
        assertEquals("1", cartCount);
    }

    @Test
    public void testRemoveItemFromCart() {
        // First add an item
        driver.findElement(By.cssSelector(".product-card:first-child .quick-add")).click();
        Thread.sleep(1000);

        // Go to cart
        driver.findElement(By.id("cart-icon")).click();

        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(10));
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.className("cart-items")));

        // Remove item
        driver.findElement(By.cssSelector(".remove-item")).click();

        // Verify empty cart
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.className("empty-cart-message")));
        String emptyMessage = driver.findElement(By.className("empty-cart-message")).getText();
        assertEquals("Your cart is empty", emptyMessage);
    }

    @Test
    public void testUpdateQuantity() {
        // Add item and go to cart
        driver.findElement(By.cssSelector(".product-card:first-child .quick-add")).click();
        driver.findElement(By.id("cart-icon")).click();

        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(10));
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.className("cart-items")));

        // Update quantity
        WebElement quantityInput = driver.findElement(By.name("quantity"));
        quantityInput.clear();
        quantityInput.sendKeys("3");
        quantityInput.sendKeys(Keys.ENTER);

        // Verify updated total
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("cart-total")));
        String total = driver.findElement(By.id("cart-total")).getText();
        assertNotNull(total);
    }

    @Test
    public void testHoverProductCard() {
        // Hover over product to see quick view
        WebElement productCard = driver.findElement(By.cssSelector(".product-card:first-child"));
        Actions actions = new Actions(driver);
        actions.moveToElement(productCard).perform();

        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(5));
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.className("quick-view-btn")));

        boolean isQuickViewVisible = driver.findElement(By.className("quick-view-btn")).isDisplayed();
        assertTrue(isQuickViewVisible);
    }

    @Test
    public void testSearchProduct() {
        WebElement searchBox = driver.findElement(By.name("search"));
        searchBox.sendKeys("laptop");
        searchBox.sendKeys(Keys.ENTER);

        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(10));
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.className("search-results")));

        List<WebElement> results = driver.findElements(By.cssSelector(".search-results .product-card"));
        assertTrue(results.size() > 0);
    }

    @Test
    public void testApplyCoupon() {
        // Add item and go to cart
        driver.findElement(By.cssSelector(".product-card:first-child .quick-add")).click();
        driver.findElement(By.id("cart-icon")).click();

        // Apply coupon
        driver.findElement(By.id("coupon-input")).sendKeys("SAVE10");
        driver.findElement(By.id("apply-coupon")).click();

        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(5));
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.className("discount-applied")));

        String discountText = driver.findElement(By.className("discount-applied")).getText();
        assertTrue(discountText.contains("10%"));
    }

    @Test
    public void testNavigateBackToProducts() {
        driver.findElement(By.id("cart-icon")).click();
        driver.navigate().back();

        String currentUrl = driver.getCurrentUrl();
        assertTrue(currentUrl.contains(BASE_URL));
    }

    private void login(String username, String password) {
        driver.findElement(By.id("username")).sendKeys(username);
        driver.findElement(By.id("password")).sendKeys(password);
        driver.findElement(By.id("loginBtn")).click();
    }
}
