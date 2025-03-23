using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using static SoftwareQualityAssurance_FoodAndDrink.LoginTest;

namespace SoftwareQualityAssurance_FoodAndDrink;

public class LogoutTest
{
    private IWebDriver driver;
    private WebDriverWait wait;

    [SetUp]
    public void SetUp()
    {
        driver = new ChromeDriver();
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        driver.Manage().Window.Maximize();

    }
    public void Login()
    {
        driver.Navigate().GoToUrl("https://localhost:44379/account/login");
        IWebElement txtEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='UserName']")));
        txtEmail.SendKeys("example@gmail.com");
        Thread.Sleep(2000);
        IWebElement txtPass = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='Password']")));
        txtPass.SendKeys("strongPassword*12345");

        Thread.Sleep(2000);
        IWebElement btn_Login = wait.Until(ExpectedConditions.ElementToBeClickable(driver.FindElement(By.XPath("//*[@id='loginForm']/form/div[4]/div/input"))));
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn_Login);
        btn_Login.Click();


    }
    #region TC3.1: Đăng xuất thành công
    [Test]
    public void LogoutSuccess()
    {

        Login();
        IWebElement dropdown = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("dropdown")));


        Actions action = new Actions(driver);
        action.MoveToElement(dropdown).Perform();
        IWebElement btn_logout = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'Đăng xuất')]")));
        btn_logout.Click();

    }
    #endregion

    #region TC3.2: Đăng xuất trạng thái phiên

    [Test]
    public void LogoutMultipleSession()
    {
        Login();

        IWebElement dropdown = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("dropdown")));
        Actions action = new Actions(driver);
        action.MoveToElement(dropdown).Perform();

        IWebElement btn_logout = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'Đăng xuất')]")));
        btn_logout.Click();

        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//a[contains(text(),'Đăng xuất')]")));

        // Find the dropdown again to avoid stale element exception
        dropdown = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("dropdown")));
        action.MoveToElement(dropdown).Perform();

        IWebElement btn_Login = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'Đăng nhập')]")));
        btn_Login.Click();

        IWebElement btn_LoginSubmit = wait.Until(ExpectedConditions.ElementToBeClickable(driver.FindElement(By.XPath("//*[@id='loginForm']/form/div[4]/div/input"))));
        btn_LoginSubmit.Click();
        IWebElement txtErrorEmail = driver.FindElement(By.XPath("//span[@data-valmsg-for='UserName']"));
        IWebElement txtErrorPass = driver.FindElement(By.XPath("//span[@data-valmsg-for='Password']"));
        TestContext.WriteLine(txtErrorEmail.Text);
        TestContext.WriteLine(txtErrorPass.Text);

    }
    #endregion

    #region TC3.3: Logout in multiple tabs
    [Test]
    public void LogoutMultipleTabs()
    {
        driver.Navigate().GoToUrl("https://localhost:44379/Account/Login");
        Login();
        // Mở tab mới
        ((IJavaScriptExecutor)driver).ExecuteScript("window.open('https://localhost:44379/Account/Login', '_blank');");

        // Lấy danh sách tất cả tab
        List<string> tabs = new List<string>(driver.WindowHandles);



        driver.SwitchTo().Window(tabs[1]);
        Login();
        IWebElement dropdown = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("dropdown")));
        Actions action = new Actions(driver);
        action.MoveToElement(dropdown).Perform();
        IWebElement btn_logout = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'Đăng xuất')]")));
        btn_logout.Click();
        Thread.Sleep(2000);
        driver.SwitchTo().Window(tabs[0]);
        driver.Navigate().Refresh();
        Thread.Sleep(2000);



    }
    #endregion

    #region TC3.4: Logout trạng thái phiên người dùng
    [Test]
    public void LogoutMultipleUsers()
    {
        Login();
        IWebElement dropdown = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("dropdown")));
        Actions action = new Actions(driver);
        action.MoveToElement(dropdown).Perform();
        IWebElement btn_logout = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'Đăng xuất')]")));
        btn_logout.Click();
        driver.Manage().Cookies.DeleteAllCookies();

        driver.Navigate().Back();
        Thread.Sleep(2000);
        driver.Navigate().Refresh();

        try
        {

            IWebElement userButton = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(text(),'Chào example@gmail.com')]")));
        }
        catch
        {
            driver.Close();
            TestContext.WriteLine("Tài khoản đã đăng xuất");
        }



    }
    #endregion

    [TearDown]
    public void TearDown()
    {
        driver.Dispose();
    }
}
