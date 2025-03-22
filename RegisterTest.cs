using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace SoftwareQualityAssurance_FoodAndDrink
{
    public class RegisterTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        private const string BASE_URL = "https://localhost:44379/";
        private const string HOME_PAGE_URL = "https://localhost:44379/trang-chu";
        private const string EXCEL_FILE_PATH = "TestData/DataEntry.xlsx"; // Đường dẫn tương đối từ thư mục gốc
        private const string SHEET_NAME = "Register Data";
        private const int ERROR_COLUMN = 9;
        private const int FIRST_DATA_ROW = 2;

        [SetUp]
        public void SetUp()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(40));
        }

        public class UserRegister
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
            public int RowIndex { get; set; }
        }

        public static IEnumerable<object[]> GetUsersFromExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", EXCEL_FILE_PATH);

            using var package = new ExcelPackage(new FileInfo(fullPath));
            var worksheet = package.Workbook.Worksheets[SHEET_NAME];
            var users = new List<object[]>();

            for (int row = FIRST_DATA_ROW; row <= 13; row++)
            {
                string data = worksheet.Cells[row, 8].Text;
                string[] lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length < 3)
                    throw new FormatException($"Row {row} has invalid format. Expected 3 lines, found {lines.Length}.");

                users.Add(new object[]
                {
                    new UserRegister
                    {
                        Email = ExtractValue(lines[0]),
                        Password = ExtractValue(lines[1]),
                        ConfirmPassword = ExtractValue(lines[2]),
                        RowIndex=row
                    }
                });
            }
            return users;
        }

        private static string ExtractValue(string line) => line.Split('"')[1];

        [TestCaseSource(nameof(GetUsersFromExcel))]
        public void VerifyRegisterFlow(UserRegister user)
        {
            NavigateToRegisterPage();
            FillRegistrationForm(user);
            SubmitRegistrationForm();

            string resultMessage = IsRegistrationSuccessful()
                ? "Success"
                : GetErrorMessage();

            TestContext.WriteLine(resultMessage);
            WriteResultToExcel(user, resultMessage);
        }

        private void NavigateToRegisterPage()
        {
            driver.Navigate().GoToUrl(BASE_URL);
            var dropdown = driver.FindElement(By.ClassName("dropdown"));
            new Actions(driver).MoveToElement(dropdown).Perform();
            var registerButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[href='/account/Register']")));
            registerButton.Click();
        }

        private void FillRegistrationForm(UserRegister user)
        {
            driver.FindElement(By.Id("Email")).SendKeys(user.Email);
            driver.FindElement(By.Id("Password")).SendKeys(user.Password);
            driver.FindElement(By.Id("ConfirmPassword")).SendKeys(user.ConfirmPassword);
        }

        private void SubmitRegistrationForm()
        {
            string initialUrl = driver.Url;
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Đăng ký']")).Submit();
        }

        private bool IsRegistrationSuccessful() => driver.Url == HOME_PAGE_URL;

        private string GetErrorMessage()
        {
            Thread.Sleep(1000);
            var errorElement = driver.FindElement(By.XPath("//div[contains(@class,'text-danger')]//li"));
            return ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].textContent;", errorElement).ToString();
        }

        private void WriteResultToExcel(UserRegister user, string result)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", EXCEL_FILE_PATH);

            using var package = new ExcelPackage(new FileInfo(fullPath));
            var worksheet = package.Workbook.Worksheets[SHEET_NAME];
            worksheet.Cells[user.RowIndex, ERROR_COLUMN].Value = result;
            package.Save();
        }

        private int FindRowForUser(ExcelWorksheet worksheet, string email)
        {
            for (int row = FIRST_DATA_ROW; row <= 13; row++)
            {
                string data = worksheet.Cells[row, 8].Text;
                if (data.Contains(email))
                    return row;
            }
            throw new Exception($"User with email {email} not found in Excel.");
        }

        [TearDown]
        public void TearDown()
        {
            driver?.Quit();
            driver?.Dispose();
        }
    }
}