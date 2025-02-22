using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SoftwareQualityAssurance_FoodAndDrink
{
    internal class SearchProduct
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
        [Test]
        public void SearchAndSelectProduct()
        {
            driver.Navigate().GoToUrl("https://localhost:44379/san-pham");

            //icon search
            var searchButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".fa.fa-search")));
            searchButton.Click();

            //input info into search bar
            var searchBox = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("search-input")));
            searchBox.Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(50));
            searchBox.SendKeys("Cà phê");
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(50));
            // gợi ý

            var firstSuggestion = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".suggestion-item:nth-child(1)")));


            //// firstSuggestion.Click();
            // var productLink = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.LinkText("Cà phê Cappuccino")));

            Thread.Sleep(5000);

        }

        [Test]
        public void SearchProductDoeasntHave()
        {

            driver.Navigate().GoToUrl("https://localhost:44379/san-pham");

            var searchButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".fa.fa-search")));
            searchButton.Click();

            //input info into search bar
            var searchBox = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("search-input")));
            searchBox.Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(50));
            searchBox.SendKeys("aa");
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(50));
           
            //< button class="btn btn-outline-secondary" type="submit">Search</button>
            searchButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[@type='submit' and contains(text(),'Search')]")));

            // Click vào nút "Search"
            searchButton.Click();
            Thread.Sleep(2000);
            //Alert
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//p[contains(text(),'Không tìm thấy sản phẩm nào phù hợp.')]")));

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
