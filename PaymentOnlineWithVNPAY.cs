using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO;
using System.Threading;
namespace SoftwareQualityAssurance_FoodAndDrink
{
    public class PaymentOnlineWithVNPAY
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string excelFilePath = "PaymentVNPAY.xlsx"; // Đường dẫn file Excel

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        [Test]
        public void RunTestsFromExcel()
        {
            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Lấy sheet đầu tiên
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // Bắt đầu từ hàng 2 
                {
                    string username = worksheet.Cells[row, 1].Text;
                    string password = worksheet.Cells[row, 2].Text;
                    string productName = worksheet.Cells[row, 3].Text;
                    string sizeId = worksheet.Cells[row, 4].Text;
                    string toppingId = worksheet.Cells[row, 5].Text;
                    string customerName = worksheet.Cells[row, 6].Text;
                    string phone = worksheet.Cells[row, 7].Text;
                    string address = worksheet.Cells[row, 8].Text;
                    string province = worksheet.Cells[row, 9].Text;
                    string district = worksheet.Cells[row, 10].Text;
                    string ward = worksheet.Cells[row, 11].Text;
                    string storeAddress = worksheet.Cells[row, 12].Text;
                    try
                    {
                        driver.Navigate().GoToUrl("https://localhost:44379/");
                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

                        Actions action = new Actions(driver);
                        var userIcon = driver.FindElement(By.CssSelector(".dropdown img"));
                        action.MoveToElement(userIcon).Click().Perform();

                        IWebElement loginLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Đăng nhập")));
                        loginLink.Click();
                        Login(username, password);
                        SelectStore(province, district, ward, address);
                        AddProductToCart(productName, sizeId, toppingId);
                        Checkout(customerName, phone, address);

                        worksheet.Cells[row, 13].Value = "PASS"; 
                    }
                    catch (Exception ex)
                    {
                        worksheet.Cells[row, 13].Value = "FAIL";
                        worksheet.Cells[row, 14].Value = ex.Message; 
                    }
                }
                package.Save();
            }
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
        private void SelectDropdown(By locator, string value)
        {

            //var dropdown = driver.FindElement(locator);
            //dropdown.FindElement(By.XPath($"//option[. = '{value}']")).Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var dropdown = wait.Until(ExpectedConditions.ElementToBeClickable(locator));
            dropdown.Click();

            var option = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath($"//option[normalize-space(.) = '{value}']")));
            option.Click();
        }

        private void Login(string username, string password)
        {
       

            driver.FindElement(By.Id("UserName")).SendKeys(username);
            driver.FindElement(By.Id("Password")).SendKeys(password);
            driver.FindElement(By.CssSelector(".btn-primary")).Click();
        }

        private void AddProductToCart(string productName, string sizeId, string toppingId)
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(productName))).Click();
            driver.FindElement(By.Id(sizeId)).Click();
            driver.FindElement(By.Id(toppingId)).Click();
            driver.FindElement(By.CssSelector(".btnAddToCart")).Click();
            driver.SwitchTo().Alert().Accept();
        }

        private void Checkout(string customerName, string phone, string address)
        {
            driver.FindElement(By.Id("checkout_items")).Click();
            driver.FindElement(By.LinkText("Thanh toán")).Click();

            driver.FindElement(By.Name("CustomerName")).SendKeys(customerName);
            driver.FindElement(By.Name("Phone")).SendKeys(phone);
            driver.FindElement(By.Name("Address")).SendKeys(address);
            driver.FindElement(By.Id("btnCheckOut")).Click();
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
