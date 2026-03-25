package com.example.tests;

import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;
import org.testng.Assert;
import org.testng.annotations.AfterMethod;
import org.testng.annotations.BeforeMethod;
import org.testng.annotations.Test;

import java.time.Duration;

public class LoginPageTest {

    private WebDriver driver;
    private String baseUrl = "https://example.com";

    @BeforeMethod
    public void setUp() {
        driver = new ChromeDriver();
        driver.manage().window().maximize();
        driver.manage().timeouts().implicitlyWait(Duration.ofSeconds(10));
        driver.get(baseUrl + "/login");
    }

    @AfterMethod
    public void tearDown() {
        if (driver != null) {
            driver.quit();
        }
    }

    @Test
    public void testLoginWithValidCredentials() {
        WebElement usernameField = driver.findElement(By.id("username"));
        usernameField.clear();
        usernameField.sendKeys("testuser");

        WebElement passwordField = driver.findElement(By.id("password"));
        passwordField.clear();
        passwordField.sendKeys("testpass123");

        driver.findElement(By.cssSelector("button[type='submit']")).click();

        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(10));
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("dashboard")));

        String pageTitle = driver.getTitle();
        Assert.assertEquals(pageTitle, "Dashboard");

        String currentUrl = driver.getCurrentUrl();
        Assert.assertTrue(currentUrl.contains("/dashboard"));
    }

    @Test
    public void testLoginWithInvalidCredentials() {
        driver.findElement(By.id("username")).sendKeys("invaliduser");
        driver.findElement(By.id("password")).sendKeys("wrongpass");
        driver.findElement(By.id("loginBtn")).click();

        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(5));
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.className("error-message")));

        String errorText = driver.findElement(By.className("error-message")).getText();
        Assert.assertEquals(errorText, "Invalid username or password");
    }

    @Test
    public void testEmptyFieldValidation() {
        driver.findElement(By.cssSelector("button[type='submit']")).click();

        WebElement usernameError = driver.findElement(By.id("username-error"));
        Assert.assertTrue(usernameError.isDisplayed());

        WebElement passwordError = driver.findElement(By.id("password-error"));
        Assert.assertTrue(passwordError.isDisplayed());
    }

    @Test
    public void testForgotPasswordLink() {
        driver.findElement(By.linkText("Forgot Password?")).click();

        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(5));
        wait.until(ExpectedConditions.urlContains("/forgot-password"));

        Assert.assertTrue(driver.getCurrentUrl().contains("/forgot-password"));
    }

    @Test
    public void testRememberMeCheckbox() {
        WebElement checkbox = driver.findElement(By.id("rememberMe"));
        Assert.assertFalse(checkbox.isSelected());

        checkbox.click();
        Assert.assertTrue(checkbox.isSelected());
    }
}
