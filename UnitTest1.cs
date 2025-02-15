using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using Newtonsoft.Json;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;


namespace SoftwareQualityAssurance_FoodAndDrink
{
    public class LoginTest
    {
        private IWebDriver driver;

        public class User
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
        }

        [Test]
        [TestCaseSource(nameof(GetUsers))]
        public void TestLogin(User user)
        {
            driver.Navigate().GoToUrl("https://localhost:44379/");
           

            // Di chuyển đến dropdown và nhấn nút đăng nhập
            IWebElement dropdown = driver.FindElement(By.ClassName("dropdown"));
            Actions actions = new Actions(driver);
            actions.MoveToElement(dropdown).Perform();
            driver.FindElement(By.CssSelector("a[href='/account/login']")).Click();
           

            // Điền thông tin đăng nhập
            driver.FindElement(By.Id("UserName")).SendKeys(user.UserName);
            driver.FindElement(By.Id("Password")).SendKeys(user.Password);
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Đăng nhập']")).Submit();
           

        }

        public static IEnumerable<User> GetUsers()
        {
            //string filePath = "C:\\Users\\Laptop\\source\\repos\\SoftwareQualityAssurance_FoodAndDrink\\SoftwareQualityAssurance_FoodAndDrink\\userData.json"; // Đường dẫn tệp JSON
            string filePath = "C:\\Users\\Phung\\source\\repos\\SoftwareQualityAssurance_FoodAndDrink\\userData.json"; // Đường dẫn tệp JSON
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<User>>(json);
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
