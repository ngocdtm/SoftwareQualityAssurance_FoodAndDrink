using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SoftwareQualityAssurance_FoodAndDrink
{
    public class CategorySuccessTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Test]//Category have product => Passed
        public void CategorySuccess()
        {
            driver.Navigate().GoToUrl("https://localhost:44379/");

            //click vào sp menu
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".super_container:nth-child(45) .large-menu-text:nth-child(2) > a"))).Click();

            //category "Cà phê"
            wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Cà phê"))).Click();

            //danh mục "Trà"
            IWebElement teaElement = wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Trà")));
            Actions actions = new Actions(driver);
            actions.MoveToElement(teaElement).Click().Perform();

            //category "Sinh tố"
            wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Sinh tố"))).Click();
        }
        [Test]//Category haven't product => Passed
        public void CategoryHaveNotSuccess()
        {
            driver.Navigate().GoToUrl("https://localhost:44379/");

            //click vào sp menu
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".super_container:nth-child(45) .large-menu-text:nth-child(2) > a"))).Click();

            //category "Yogurt"
            wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Yogurt"))).Click();

        }

        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }
    }
}
