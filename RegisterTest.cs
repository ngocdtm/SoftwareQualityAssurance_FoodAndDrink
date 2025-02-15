using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.Modules.Script;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static SoftwareQualityAssurance_FoodAndDrink.LoginTest;

namespace SoftwareQualityAssurance_FoodAndDrink
{
    [TestClass]
    public class RegisterTest
    {
        private IWebDriver driver;
        public class User_Register
        {
            public string email { get; set; }
            public string password { get; set; }
            public string confirmPassword { get; set; }
        }
        [SetUp]
        public void SetUp()
        {
            Console.WriteLine("Starting WebDriver setup...");
            driver = new ChromeDriver();
            Console.WriteLine("WebDriver initialized.");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }
        [Test]
        [TestCaseSource(nameof(GetUserRegister))]
        public void TestRegister(User_Register user_Register)
        {
            driver.Navigate().GoToUrl("https://localhost:44379/");

            // Navigate to dropdown and click register
            IWebElement dropdown = driver.FindElement(By.ClassName("dropdown"));
            Actions actions = new Actions(driver);
            actions.MoveToElement(dropdown).Perform();
            driver.FindElement(By.CssSelector("a[href='/account/Register']")).Click();
            //wait for 
            WebDriverWait wait=new WebDriverWait(driver,TimeSpan.FromSeconds(10));

            //fill textfield
            driver.FindElement(By.Id("Email")).SendKeys(user_Register.email);
            driver.FindElement(By.Id("Password")).SendKeys(user_Register.password);
            driver.FindElement(By.Id("ConfirmPassword")).SendKeys(user_Register.confirmPassword);
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Đăng ký']")).Submit();
            Thread.Sleep(2000);
            //check if register successfully -> write to userData.json -- update later
            bool isValid = false;
           

        }
        public static IEnumerable<User_Register> GetUserRegister()
        {
            string filePath = "C:\\Users\\Phung\\source\\repos\\SoftwareQualityAssurance_FoodAndDrink\\registerInfor.json"; // Đường dẫn tệp JSON
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<User_Register>>(json);
        }

        [TearDown]  // Runs after each test
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
