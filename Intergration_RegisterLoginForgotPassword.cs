using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers; //ExpectedConditions


namespace SoftwareQualityAssurance_FoodAndDrink;

public class Intergration_RegisterLoginForgotPassword
{
    private IWebDriver driver;
    private WebDriverWait wait;
    [SetUp]
    public void SetUp()
    {
        driver = new ChromeDriver();
        driver.Manage().Window.Maximize();

    }
    static string EMAIL = "nghkphung95@gmail.com";
    static string REPASS = "newPassword#123";
    string RESET_LINK_FILE_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "TestData", "reset_link.txt"); 
    [Test]
    public void test()
    {
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
     
        string loginUrl = "https://localhost:44379/account/login";
        driver.Navigate().GoToUrl(loginUrl);
        IWebElement txtEmailLogin = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='UserName']")));
        txtEmailLogin.SendKeys(EMAIL);
        Thread.Sleep(1000);
        IWebElement btn_forgotPass = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[@href='/account/ForgotPassword']")));
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn_forgotPass);
        btn_forgotPass.Click();
        wait.Until(d => d.Url != loginUrl);
        IWebElement txtForgotEmail = wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@id='Email']")));
        txtForgotEmail.SendKeys(EMAIL);
        Thread.Sleep(1000);
        IWebElement btn_forgotLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//input[@type='submit' and @value='Email Link']")));
        btn_forgotLink.Click();
        wait.Until(ExpectedConditions.UrlToBe("https://localhost:44379/Account/ForgotPasswordConfirmation"));

        // Đọc link từ file
        string resetLink = GetResetLinkFromFile();
        if (string.IsNullOrEmpty(resetLink))
        {
            throw new Exception("Không tìm thấy link reset password trong file.");
        }
        driver.Navigate().GoToUrl(resetLink);

        IWebElement reMail = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#Email")));
        reMail.SendKeys(EMAIL);
        Thread.Sleep(1000);

        IWebElement rePass = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#Password")));
        rePass.SendKeys(REPASS);
        Thread.Sleep(1000);

        IWebElement reConfirmPass = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[name='ConfirmPassword']")));
        reConfirmPass.SendKeys(REPASS);
        Thread.Sleep(1000);

        IWebElement btn_reset = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input.btn.btn-outline-dark[type='submit'][value='Reset']")));
        btn_reset.Submit();
        Thread.Sleep(1000);

        //Login with repass
        ReLogin();

        //verify welcome button
        IWebElement btn_welcome = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".dropbtn")));
        string buttonText = btn_welcome.Text; 
       
        
        if (buttonText.Contains(EMAIL))
        {
            Assert.Pass();

        }
        else
        {
            Assert.Fail();
        }
        driver.Quit();


    }
    //cần cập nhật lại link vì link reset password chứa token thay đổi liên tục
    private string GetResetLinkFromFile()
    {
        // Chờ file được ghi
        int maxAttempts = 10;
        int attempt = 0;
        while (!File.Exists(RESET_LINK_FILE_PATH) && attempt < maxAttempts)
        {
            Thread.Sleep(5000); 
            attempt++;
        }

        if (!File.Exists(RESET_LINK_FILE_PATH))
        {
            throw new Exception($"File {RESET_LINK_FILE_PATH} không tồn tại sau khi chờ.");
        }

        return File.ReadAllText(RESET_LINK_FILE_PATH).Trim();
    }
    private void ReLogin()
    {
        driver.Navigate().GoToUrl("https://localhost:44379/");
        IWebElement dropdown = driver.FindElement(By.ClassName("dropdown"));
        Actions action = new Actions(driver);
        action.MoveToElement(dropdown).Perform();
        IWebElement loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[href='/account/login']")));
        loginButton.Click();
        IWebElement email = driver.FindElement(By.XPath("//*[@id='UserName']"));
        email.SendKeys(EMAIL);
        Thread.Sleep(2000);
        IWebElement password = driver.FindElement(By.XPath("//*[@id='Password']"));
        password.SendKeys(REPASS);
        IWebElement btn_Login = wait.Until(ExpectedConditions.ElementToBeClickable(driver.FindElement(By.XPath("//*[@id='loginForm']/form/div[4]/div/input"))));
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn_Login);
        btn_Login.Click();
    }

    [TearDown]
    public void TearDown()
    {
        driver.Dispose();
    }

}
