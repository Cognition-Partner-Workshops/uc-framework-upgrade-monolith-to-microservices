package com.example.tests;

import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.Select;
import org.openqa.selenium.support.ui.WebDriverWait;
import org.testng.Assert;
import org.testng.annotations.AfterMethod;
import org.testng.annotations.BeforeMethod;
import org.testng.annotations.Test;

import java.time.Duration;

public class FormSubmissionTest {

    private WebDriver driver;
    private String baseUrl = "https://example.com";

    @BeforeMethod
    public void setUp() {
        driver = new ChromeDriver();
        driver.manage().window().maximize();
        driver.get(baseUrl + "/register");
    }

    @AfterMethod
    public void tearDown() {
        if (driver != null) {
            driver.quit();
        }
    }

    @Test
    public void testSuccessfulRegistration() {
        // Fill in registration form
        driver.findElement(By.id("firstName")).sendKeys("John");
        driver.findElement(By.id("lastName")).sendKeys("Doe");
        driver.findElement(By.id("email")).sendKeys("john.doe@example.com");
        driver.findElement(By.id("phone")).sendKeys("1234567890");

        // Select country from dropdown
        new Select(driver.findElement(By.id("country"))).selectByVisibleText("United States");

        // Select gender radio button
        driver.findElement(By.cssSelector("input[value='male']")).click();

        // Accept terms checkbox
        driver.findElement(By.id("terms")).click();

        // Upload profile picture
        driver.findElement(By.id("profilePic")).sendKeys("/path/to/image.jpg");

        // Submit form
        driver.findElement(By.xpath("//button[@type='submit']")).click();

        // Wait for success message
        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(10));
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.className("success-message")));

        String successText = driver.findElement(By.className("success-message")).getText();
        Assert.assertEquals(successText, "Registration successful!");
        Assert.assertTrue(driver.getCurrentUrl().contains("/welcome"));
    }

    @Test
    public void testEmailValidation() {
        driver.findElement(By.id("email")).sendKeys("invalid-email");
        driver.findElement(By.xpath("//button[@type='submit']")).click();

        WebElement emailError = driver.findElement(By.id("email-error"));
        Assert.assertTrue(emailError.isDisplayed());
        Assert.assertEquals(emailError.getText(), "Please enter a valid email address");
    }

    @Test
    public void testPasswordStrength() {
        // Test weak password
        driver.findElement(By.id("password")).sendKeys("123");
        WebElement strengthIndicator = driver.findElement(By.id("password-strength"));
        Assert.assertEquals(strengthIndicator.getAttribute("class"), "weak");

        // Test strong password
        driver.findElement(By.id("password")).clear();
        driver.findElement(By.id("password")).sendKeys("Str0ng!P@ssw0rd");
        Assert.assertEquals(strengthIndicator.getAttribute("class"), "strong");
    }

    @Test
    public void testFormReset() {
        // Fill some fields
        driver.findElement(By.id("firstName")).sendKeys("John");
        driver.findElement(By.id("lastName")).sendKeys("Doe");

        // Click reset button
        driver.findElement(By.id("resetBtn")).click();

        // Verify fields are cleared
        String firstName = driver.findElement(By.id("firstName")).getAttribute("value");
        String lastName = driver.findElement(By.id("lastName")).getAttribute("value");
        Assert.assertEquals(firstName, "");
        Assert.assertEquals(lastName, "");
    }

    @Test
    public void testMultiStepForm() {
        // Step 1: Personal info
        driver.findElement(By.id("firstName")).sendKeys("John");
        driver.findElement(By.id("lastName")).sendKeys("Doe");
        driver.findElement(By.id("nextStep1")).click();

        // Wait for step 2
        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(5));
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("step2")));

        // Step 2: Contact info
        driver.findElement(By.id("email")).sendKeys("john@example.com");
        driver.findElement(By.id("phone")).sendKeys("9876543210");
        driver.findElement(By.id("nextStep2")).click();

        // Wait for step 3
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("step3")));

        // Step 3: Review and submit
        driver.findElement(By.id("submitForm")).click();

        // Verify success
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.className("success-message")));
        Assert.assertNotNull(driver.findElement(By.className("success-message")));
    }
}
