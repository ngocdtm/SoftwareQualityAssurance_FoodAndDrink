using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
namespace SoftwareQualityAssurance_FoodAndDrink
{
    public class ProductDetail
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

        [Test]//product have full detail product=> Passed
        public void DetailProductFullSuccess()
        {

            driver.Navigate().GoToUrl("https://localhost:44379/");


            //click vào sp menu
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".super_container:nth-child(45) .large-menu-text:nth-child(2) > a"))).Click();
            //Hover item product
            var productElement = driver.FindElement(By.ClassName("product_filter"));
            //Name Product
            var productName = productElement.FindElement(By.CssSelector(".product_name a")).Text;//actual
            Assert.That(productName, Is.EqualTo("Cà phê Cappuccino"), "Tên sản phẩm không đúng");
            //Price Product
            var productPrice = productElement.FindElement(By.ClassName("product_price")).Text;
            Assert.That(productPrice, Is.EqualTo("38.000"), "Giá sản phẩm không đúng");

            //Sale Label
            var saleTags = driver.FindElements(By.CssSelector(".product_bubble.product_bubble_red"));
            Assert.That(saleTags.Count, Is.GreaterThan(0), "Sản phẩm không có nhãn sale");
        }

        [Test]//Detail Product Must Choose Branch then can view detail product=> Passed
        public void DetailProductMustChooseBranch()
        {

            driver.Navigate().GoToUrl("https://localhost:44379/");
            //click vào sp menu
            driver.FindElement(By.CssSelector(".super_container:nth-child(45) .large-menu-text:nth-child(2) > a")).Click();
            //Product Name click
            driver.FindElement(By.LinkText("Cà phê Latte")).Click();
            //Container alert
            var warningMessage = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("alert-warning")));

            //Alert
            Assert.That(warningMessage.Text, Is.EqualTo("Vui lòng chọn cửa hàng trước khi xem chi tiết sản phẩm."),
                        "Thông báo cảnh báo không đúng hoặc không hiển thị.");


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
