using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SoftwareQualityAssurance_FoodAndDrink
{
    public class AddProductToCart
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
        public void IntergrationManageCartAndManageOrder()
        {
            driver.Navigate().GoToUrl("https://localhost:44379/");


            Login("22dh112391@gmail.com", "123asd!@#ASD");
            SelectStore("Thành phố Hồ Chí Minh", "Quận 10", "Phường 12", "h Tan Thoi");
            AddProduct("Cà phê Latte", "size_13", "topping_28");
            Checkout("Đặng Thị Mỹ Ngọc", "0378432963", "h Tan Thoi");
        }

        private void Login(string username, string password)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            Actions action = new Actions(driver);
            var userIcon = driver.FindElement(By.CssSelector(".dropdown img"));
            action.MoveToElement(userIcon).Click().Perform();

            IWebElement loginLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Đăng nhập")));
            loginLink.Click();

            driver.FindElement(By.Id("UserName")).SendKeys(username);
            driver.FindElement(By.Id("Password")).SendKeys(password);
            driver.FindElement(By.CssSelector(".btn-primary")).Click();
        }

        private void SelectStore(string province, string district, string ward, string address)
        {
            driver.FindElement(By.CssSelector(".super_container:nth-child(45) .large-menu-text:nth-child(2) > a")).Click();
            driver.FindElement(By.Id("open-store-modal")).Click();

            SelectDropdown(By.Id("province"), province);
            SelectDropdown(By.Id("district"), district);
            SelectDropdown(By.Id("ward"), ward);

            driver.FindElement(By.Id("address-line")).SendKeys(address);
            driver.FindElement(By.Id("save-address")).Click();
            driver.FindElement(By.Id("find-stores")).Click();
            Thread.Sleep(1000);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            IWebElement specificCard = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//h5[text()='Chi nhánh Matane Sư Vạn Hạnh']/ancestor::a")));

            Thread.Sleep(100);
            specificCard.Click();

        }

        private void AddProduct(string productName, string sizeId, string toppingId)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            Thread.Sleep(2000);
            wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(productName))).Click();
            Thread.Sleep(2000);
            driver.FindElement(By.Id(sizeId)).Click();
            driver.FindElement(By.Id(toppingId)).Click();
            Thread.Sleep(2000);
            driver.FindElement(By.CssSelector(".btnAddToCart")).Click();
            Thread.Sleep(2000);
            Assert.That(driver.SwitchTo().Alert().Text, Is.EqualTo("Thêm sản phẩm vào giỏ hàng thành công!"));
            driver.SwitchTo().Alert().Accept();
        }

        private void Checkout(string customerName, string phone, string address)
        {
            driver.FindElement(By.Id("checkout_items")).Click();
        }

        private void SelectDropdown(By locator, string value)
        {

          

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var dropdown = wait.Until(ExpectedConditions.ElementToBeClickable(locator));
            dropdown.Click();

            var option = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath($"//option[normalize-space(.) = '{value}']")));
            option.Click();
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
