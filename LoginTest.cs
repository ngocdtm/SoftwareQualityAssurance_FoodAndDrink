
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using OfficeOpenXml;
using static SoftwareQualityAssurance_FoodAndDrink.LoginTest;

namespace SoftwareQualityAssurance_FoodAndDrink
{

    public class LoginTest
    {
        private WebDriver driver;
        private WebDriverWait wait;
        private List<string> errorMessage;
        private int i = 1; //initial row in excel
        [SetUp]
        public void SetUp()
        {
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30); // Ensure page load timeout
        }
        public class User_Login
        {
            public string email { get; set; }
            public string password { get; set; }
        }
        #region TC 2.1-> TC 2.7
        [TestCaseSource(nameof(readFileFromExcel))]
        public void TestLogin(User_Login user_Login)
        {
            driver.Navigate().GoToUrl("https://localhost:44379/");
            string initialUrl = driver.Url;
            errorMessage = new List<string>();
            //drop down
            IWebElement dropdown = driver.FindElement(By.ClassName("dropdown"));
            Actions action = new Actions(driver);
            action.MoveToElement(dropdown).Perform();
            IWebElement loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[href='/account/login']")));
            loginButton.Click();
            IWebElement email = driver.FindElement(By.XPath("//*[@id='UserName']"));
            email.SendKeys(user_Login.email);
            IWebElement password = driver.FindElement(By.XPath("//*[@id='Password']"));
            password.SendKeys(user_Login.password);
            IWebElement btn_Login = wait.Until(ExpectedConditions.ElementToBeClickable(driver.FindElement(By.XPath("//*[@id='loginForm']/form/div[4]/div/input"))));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn_Login);
            btn_Login.Click();
            //TC 2.1: Login thanh cong
            if (driver.Url != initialUrl && driver.Url.Equals("https://localhost:44379/trang-chu"))
            {
                IWebElement btn_welcome = driver.FindElement(By.XPath($"//button[contains(text(),'Chào {user_Login.email}')]"));
                errorMessage.Add("Login success!");
            }
            else
            {
                IReadOnlyCollection<IWebElement> elements = driver.FindElements(By.XPath("//div[contains(@class,'text-danger')]//li"));
                if (elements.Count > 0)
                {
                    IWebElement element = elements.First();
                    errorMessage.Add((string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].textContent;", element));
                }
                else
                {
                    if (string.IsNullOrEmpty(email.Text) && string.IsNullOrEmpty(password.Text))
                    {
                        IWebElement txtErrorEmail = driver.FindElement(By.XPath("//span[@data-valmsg-for='UserName']"));
                        IWebElement txtErrorPass = driver.FindElement(By.XPath("//span[@data-valmsg-for='Password']"));
                        errorMessage.Add(txtErrorEmail.Text);
                        errorMessage.Add(txtErrorPass.Text);

                    }
                    else if (string.IsNullOrEmpty( email.Text))
                    {
                        IWebElement txtErrorEmail = driver.FindElement(By.XPath("//span[@data-valmsg-for='UserName']"));
                        errorMessage.Add(txtErrorEmail.Text);

                    }
                    else if (string.IsNullOrEmpty(password.Text))
                    {

                        IWebElement txtErrorPass = driver.FindElement(By.XPath("//span[@data-valmsg-for='Password']"));
                        errorMessage.Add(txtErrorPass.Text);


                    }

                }
            }
            i++;
            string allError = "";
            for (int j = 0; j < errorMessage.Count; j++)
            {
                TestContext.WriteLine(errorMessage[j].ToString());
                allError += errorMessage[j].ToString();
            }
            writeResultForExcel(i, allError);
        }
        #endregion


        #region Handle excel file
        private void writeResultForExcel(int i, string e)
        {

            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            string path = @"C:\Users\Phung\source\repos\SoftwareQualityAssurance_FoodAndDrink\TestData\DataEntry.xlsx";
            using (var package = new ExcelPackage(path))
            {
                string sheetName = "Login Data";
                ExcelWorksheet sheet = package.Workbook.Worksheets[sheetName];

                sheet.Cells[i, 11].Value = e;
                package.Save();

            }
        }
        public static IEnumerable<object[]> readFileFromExcel()
        {
            var login_Users = new List<object[]>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string path = @"C:\Users\Phung\source\repos\SoftwareQualityAssurance_FoodAndDrink\TestData\DataEntry.xlsx";
            using (var package = new ExcelPackage(new FileInfo(path)))
            {
                string sheetName = "Login Data";
                ExcelWorksheet sheet = package.Workbook.Worksheets[sheetName];

                for (int i = 2; i <= 8; i++)
                {
                    string data = sheet.Cells[i, 8].Text;
                    string[] lines = data.Split('\n');


                    var user = new User_Login
                    {
                        email = lines[0].Split('"')[1],
                        password = lines[1].Split('"')[1]

                    };
                    login_Users.Add(new object[] { user });

                }
            }
            return login_Users;

        }
        #endregion


        #region TC 2.8: Remember me 
        [Test]
        public void TestRememberMe()
        {
            User_Login user_Login = new User_Login()
            {
                email = "example@gmail.com",
                password = "strongPassword*12345"
            };
            driver.Navigate().GoToUrl("https://localhost:44379/");
            IWebElement dropdown = driver.FindElement(By.ClassName("dropdown"));
            Actions action = new Actions(driver);
            action.MoveToElement(dropdown).Perform();
            IWebElement loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[href='/account/login']")));
            loginButton.Click();
            IWebElement email = driver.FindElement(By.XPath("//*[@id='UserName']"));
            email.SendKeys(user_Login.email);
            IWebElement password = driver.FindElement(By.XPath("//*[@id='Password']"));
            password.SendKeys(user_Login.password);
            IWebElement checkRemember = wait.Until(ExpectedConditions.ElementToBeClickable(driver.FindElement(By.XPath("//*[@id='RememberMe']"))));
            checkRemember.Click();
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", checkRemember);
            IWebElement btn_Login = wait.Until(ExpectedConditions.ElementToBeClickable(driver.FindElement(By.XPath("//*[@id='loginForm']/form/div[4]/div/input"))));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn_Login);
            btn_Login.Click();

            driver.Quit();

            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://localhost:44379/trang-chu");
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            try
            {
                var check = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath($"//button[contains(text(),'Chào {user_Login.email}')]")));
                Assert.That(check.Displayed, Is.True);
            }
            catch
            {
                driver.Close();
                string message = "Can not remember account";
                TestContext.WriteLine(message);
                writeResultForExcel(9, message);
            }


        }
        #endregion


        #region TC 2.9: Two accounts
        [Test]
        public void LoginWith2Accounts()
        {
            User_Login user1 = new User_Login
            {
                email = "example@gmail.com",
                password = "strongPassword*12345"
            };
            User_Login user2 = new User_Login
            {
                email = "newUser@gmail.com",
                password = "strongPassword*12345"
            };
            driver.Navigate().GoToUrl("https://localhost:44379/");
            //thuc hien dang nhap voi trang dau tien
            Thread.Sleep(2000);
            Login(user1.email, user1.password);
            // Mở tab mới
            driver.Manage().Cookies.DeleteAllCookies();

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open('https://localhost:44379/', '_blank');");

            // Lấy danh sách tất cả tab
            List<string> tabs = new List<string>(driver.WindowHandles);

            driver.SwitchTo().Window(tabs[1]);
            Login(user2.email, user2.password);
            driver.SwitchTo().Window(tabs[0]);

            driver.Navigate().Refresh();
            try
            {
                IWebElement btn_User = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath($"//button[contains(text(),'Chào {user1.email}')]")));
                TestContext.WriteLine("user 1 is login");

            }
            catch
            {
                IWebElement btn_User = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath($"//button[contains(text(),'Chào {user2.email}')]")));
                string output = "User 2 is login in both tabs";
                TestContext.WriteLine(output);
                writeResultForExcel(10, output);

            };
        }
        #endregion

        public void Login(string user_email, string user_pass)
        {
            //drop down
            IWebElement dropdown = driver.FindElement(By.ClassName("dropdown"));
            Actions action = new Actions(driver);
            action.MoveToElement(dropdown).Perform();
            IWebElement loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[href='/account/login']")));
            loginButton.Click();
            Thread.Sleep(2000);
            IWebElement email = driver.FindElement(By.XPath("//*[@id='UserName']"));
            email.SendKeys(user_email);
            Thread.Sleep(2000);
            IWebElement password = driver.FindElement(By.XPath("//*[@id='Password']"));
            password.SendKeys(user_pass);
            Thread.Sleep(2000);
            IWebElement btn_Login = wait.Until(ExpectedConditions.ElementToBeClickable(driver.FindElement(By.XPath("//*[@id='loginForm']/form/div[4]/div/input"))));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn_Login);
            btn_Login.Click();

        }

        [TearDown]
        public void TearDown()
        {
            driver?.Dispose();


        }
    }
}