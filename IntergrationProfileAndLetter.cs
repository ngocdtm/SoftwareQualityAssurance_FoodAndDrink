using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SoftwareQualityAssurance_FoodAndDrink
{
    internal class IntergrationProfileAndLetter
    {
        internal class IntergrationPaymentAndCheckInAdmin
        {
            private IWebDriver driver;
            private WebDriverWait wait;

            [SetUp]
            public void Setup()
            {
                driver = new EdgeDriver();
                driver.Manage().Window.Maximize();
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            }

            [Test]
            public void IntergrationManageCartAndManageOrder()
            {
                NavigateToHomePage();


                Login("22dh112391@gmail.com", "123asd!@#ASD");
                UpdateProfile("Đặng Thị Ahihihi", "123456789", "Ho Chi Minh");
                Thread.Sleep(1000);
                CreateReview("Sth review good", "uuuu");
                Thread.Sleep(1000);
                driver.Navigate().GoToUrl("https://localhost:44379/admin/feedbackletter/index");
                Thread.Sleep(2000);
            }
            private void NavigateToHomePage()
            {
                driver.Navigate().GoToUrl("https://localhost:44379/trang-chu");
            }
            private void Login(string username, string password)
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2000));

                Actions action = new Actions(driver);
                var userIcon = driver.FindElement(By.CssSelector(".dropdown img"));
                action.MoveToElement(userIcon).Click().Perform();

                IWebElement loginLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Đăng nhập")));
                loginLink.Click();

                driver.FindElement(By.Id("UserName")).SendKeys(username);
                driver.FindElement(By.Id("Password")).SendKeys(password);
                driver.FindElement(By.CssSelector(".btn-primary")).Click();
            }

            private void UpdateProfile(string fullName, string phoneNumber, string address)
            {

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                Actions action = new Actions(driver);
                IWebElement dropdown = driver.FindElement(By.CssSelector(".dropbtn")); // Tìm nút "Chào admin"
                action.MoveToElement(dropdown).Perform(); // Hover chuột vào dropdown
                Thread.Sleep(2000);
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                IWebElement profileLink = driver.FindElement(By.XPath("//a[contains(text(),'Thông tin cá nhân')]"));
                js.ExecuteScript("arguments[0].click();", profileLink);
                // Click vào "Thông tin cá nhân"
                driver.FindElement(By.Name("FullName")).Clear();
                driver.FindElement(By.Name("FullName")).SendKeys(fullName);
                Thread.Sleep(1000);
                driver.FindElement(By.Name("PhoneNumber")).Clear();
                driver.FindElement(By.Name("PhoneNumber")).SendKeys(phoneNumber);
                Thread.Sleep(1000);
                driver.FindElement(By.Name("Address")).Clear();
                driver.FindElement(By.Name("Address")).SendKeys(address);
                driver.FindElement(By.CssSelector(".btn-success")).Click();
            }

            private void CreateReview(string title, string content)
            {
                driver.FindElement(By.CssSelector(".large-menu-text:nth-child(5) > a")).Click();
                driver.FindElement(By.Id("Title")).SendKeys(title);
                Thread.Sleep(1000);
                driver.FindElement(By.Id("Content")).SendKeys(content);
                Thread.Sleep(1000);
                driver.FindElement(By.CssSelector(".btn-success")).Click();
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
}
