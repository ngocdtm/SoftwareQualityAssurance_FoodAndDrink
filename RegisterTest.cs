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
using NUnit.Framework;
using OfficeOpenXml;
using SeleniumExtras.WaitHelpers;

namespace SoftwareQualityAssurance_FoodAndDrink
{
    public class RegisterTest
    {
        private IWebDriver driver;
        private int i = 1; //so row trong file excel
        private string errorMessage;
        [SetUp]
        public void SetUp()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();

        }
        public class User_Register
        {
            public string email { get; set; }
            public string password { get; set; }
            public string confirmPassword { get; set; }
        }

        public static IEnumerable<object[]> ReadUsersFromExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var users = new List<object[]>();
            string path = @"C:\Users\Phung\source\repos\SoftwareQualityAssurance_FoodAndDrink\TestData\DataEntry.xlsx";
            using (var package = new ExcelPackage(new FileInfo(path)))
            {
                string sheetName = "Register Data";
                ExcelWorksheet sheet = package.Workbook.Worksheets[sheetName];
                for (int row = 2; row <= 13; row++)
                {
                    string data = sheet.Cells[row, 8].Text;
                    string[] lines = data.Split('\n');
                    if (lines.Length < 3)
                    {
                        throw new Exception($"Invalid data format in row {row}. Expected at least 3 lines but found {lines.Length}");
                    }

                    var user = new User_Register
                    {
                        email = lines[0].Split('"')[1],
                        password = lines[1].Split('"')[1],
                        confirmPassword = lines[2].Split('"')[1]


                    };
                    users.Add(new object[] { user }); // MSTest requires `object[]`

                }
                return users;

                
            }
        }
       
        [TestCaseSource(nameof(ReadUsersFromExcel))]
        public void TestRegister(User_Register user_Register)
        {
            driver.Navigate().GoToUrl("https://localhost:44379/");
            string initalUrl = driver.Url;
            // Navigate to dropdown and click register
            IWebElement dropdown = driver.FindElement(By.ClassName("dropdown"));
            Actions actions = new Actions(driver);
            actions.MoveToElement(dropdown).Perform();
            WebDriverWait wait=new WebDriverWait(driver,TimeSpan.FromSeconds(10));
            IWebElement registerButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[href='/account/Register']")));
            registerButton.Click();
            //fill textfield
            driver.FindElement(By.Id("Email")).SendKeys(user_Register.email);
            Thread.Sleep(2000);

            driver.FindElement(By.Id("Password")).SendKeys(user_Register.password);
            Thread.Sleep(2000);

            driver.FindElement(By.Id("ConfirmPassword")).SendKeys(user_Register.confirmPassword);
            Thread.Sleep(2000);

            driver.FindElement(By.XPath("//input[@type='submit' and @value='Đăng ký']")).Submit();
            wait.Until(d => d.Url != initalUrl);
            //register succes -> render login pager
            if (driver.Url.Equals("https://localhost:44379/trang-chu"))
            {
                errorMessage = "Success";

            }
            else {
                //doc cac thong bao loi
                IWebElement element = driver.FindElement(By.XPath("//div[contains(@class,'text-danger')]//li"));
                IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)driver;
                errorMessage = (string)javaScriptExecutor.ExecuteScript("return arguments[0].textContent;", element);
            }
           
                TestContext.Write(errorMessage);
                i++;
                writeExcelFile(i, errorMessage); //ghi vào file các error message


        }




        public void writeExcelFile(int i,string errorMessage)
        {
            ExcelPackage.LicenseContext=LicenseContext.Commercial;
            string path = @"C:\Users\Phung\source\repos\SoftwareQualityAssurance_FoodAndDrink\TestData\DataEntry.xlsx";
            using (var package = new ExcelPackage(path))
            {
                string sheetName = "Register Data";
                ExcelWorksheet sheet = package.Workbook.Worksheets[sheetName];

                sheet.Cells[i,9].Value = errorMessage; 
                package.Save();

            }
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
