using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Newtonsoft.Json;

namespace SoftwareQualityAssurance_FoodAndDrink
{
    [TestFixture]
    public class ManageProduct
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private IJavaScriptExecutor js;
        private IDictionary<string, object> vars;

        public class ProductTestData
        {
            public string TestName { get; set; }
            public string ProductName { get; set; }
            public string Description { get; set; }
            public string Detail { get; set; }
            public string Quantity { get; set; }
            public string Price { get; set; }
            public string OriginalPrice { get; set; }
            public string ExpectedErrorMessage { get; set; }
            public string ExpectedErrorField { get; set; }
        }

        // Đọc dữ liệu test từ file JSON
        private List<ProductTestData> GetTestCases()
        {
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "AddProductTest.json");

            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException($"File dữ liệu test không tồn tại: {jsonFilePath}");
            }

            string jsonContent = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<List<ProductTestData>>(jsonContent);
        }

        [SetUp]
        public void SetUp()
        {
            // Khởi tạo driver và các biến cần thiết
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            js = (IJavaScriptExecutor)driver;
            vars = new Dictionary<string, object>();

            // Đăng nhập với quyền Admin để test
            AdminLogin();
        }

        private void AdminLogin()
        {
            driver.Navigate().GoToUrl("https://localhost:44379/Admin/Account/Login");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("UserName"))).Click();
            driver.FindElement(By.Id("UserName")).Clear();
            driver.FindElement(By.Id("UserName")).SendKeys("admin");

            driver.FindElement(By.Id("Password")).Clear();
            driver.FindElement(By.Id("Password")).SendKeys("Admin*123");

            driver.FindElement(By.CssSelector(".btn")).Click();

            // Đợi đăng nhập thành công
            wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Xem sản phẩm")));
        }

        [Test]
        [TestCaseSource(nameof(GetTestCaseNames))]
        public void TestAddProductWithInvalidData(string testName)
        {
            // Lấy dữ liệu test từ tên test case
            ProductTestData testData = GetTestCases().FirstOrDefault(t => t.TestName == testName);
            //Assert.NotNull(testData, $"Không tìm thấy test case với tên {testName}");

            Console.WriteLine($"Thực hiện test case: {testName}");

            // Các bước test
            NavigateToAddProduct();
            FillProductForm(testData);
            SubmitForm();
            VerifyError(testData.ExpectedErrorMessage, testData.ExpectedErrorField);
        }

        // Lấy danh sách tên các test case từ file JSON
        public static IEnumerable<string> GetTestCaseNames()
        {
            var testInstance = new ManageProduct();
            return testInstance.GetTestCases().Select(t => t.TestName);
        }

        private void NavigateToAddProduct()
        {
            // Điều hướng đến trang quản lý sản phẩm
            driver.FindElement(By.LinkText("Xem sản phẩm")).Click();

            // Nhấn nút thêm mới
            wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Thêm mới"))).Click();
        }

        private void FillProductForm(ProductTestData data)
        {
            // Chọn loại sản phẩm là Food
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("productTypeDropdown"))).Click();
            var dropdown = driver.FindElement(By.Id("productTypeDropdown"));
            dropdown.FindElement(By.XPath("//option[. = 'Food']")).Click();

            // Nhập tiêu đề sản phẩm
            IWebElement titleElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Title")));
            titleElement.Click();
            titleElement.Clear();
            titleElement.SendKeys(data.ProductName);
            Thread.Sleep(500);

            // Chọn extra options
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("SelectedExtraIds")));
                IWebElement selectElement = driver.FindElement(By.Id("SelectedExtraIds"));

                // Sử dụng SelectElement để thao tác với multi-select
                SelectElement multiSelect = new SelectElement(selectElement);

                // Chọn option đầu tiên
                multiSelect.SelectByValue("1"); // Chọn "Phô mai" hoặc option có value=1

                // Thêm small delay cho UI update
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Không thể chọn extras: {ex.Message}");
            }

            // Nhập mô tả
            IWebElement descriptionElement = wait.Until(ExpectedConditions.ElementExists(By.Id("Description")));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", descriptionElement);
            Thread.Sleep(500);
            descriptionElement.Click();
            descriptionElement.Clear();
            descriptionElement.SendKeys(data.Description);

            // Nhập chi tiết
            IWebElement detailElement = wait.Until(ExpectedConditions.ElementExists(By.Id("txtDetail")));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", detailElement);
            Thread.Sleep(500);
            detailElement.Click();
            detailElement.Clear();
            detailElement.SendKeys(data.Detail);

            // Nhập số lượng
            IWebElement quantityElement = wait.Until(ExpectedConditions.ElementExists(By.Id("Quantity")));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", quantityElement);
            Thread.Sleep(500);
            quantityElement.Click();
            quantityElement.Clear();
            quantityElement.SendKeys(data.Quantity);

            // Nhập giá
            IWebElement demoPriceElement = wait.Until(ExpectedConditions.ElementExists(By.Id("demoPrice")));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", demoPriceElement);
            Thread.Sleep(500);
            demoPriceElement.Clear();
            demoPriceElement.SendKeys(data.Price);

            // Nhập giá gốc
            IWebElement demoOriginalPriceElement = wait.Until(ExpectedConditions.ElementExists(By.Id("demoOriginalPrice")));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", demoOriginalPriceElement);
            Thread.Sleep(500);
            demoOriginalPriceElement.Clear();
            demoOriginalPriceElement.SendKeys(data.OriginalPrice);

            // Chọn hiển thị sản phẩm
            try
            {
                IWebElement displayCheckbox = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".custom-control-label")));
                js.ExecuteScript("arguments[0].scrollIntoView(true);", displayCheckbox);
                Thread.Sleep(500);
                displayCheckbox.Click();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Không thể click vào checkbox hiển thị: {ex.Message}");
            }
        }

        private void SubmitForm()
        {
            // Lưu sản phẩm
            IWebElement saveButton = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".btn-success")));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", saveButton);
            Thread.Sleep(500);
            saveButton.Click();

            Thread.Sleep(1000);
        }

        private void VerifyError(string expectedErrorMessage, string errorFieldId)
        {
            try
            {
                Console.WriteLine($"Đang kiểm tra lỗi: '{expectedErrorMessage}' cho trường {errorFieldId}");

                // Add a small wait to make sure validation messages have time to appear
                Thread.Sleep(1000);

                // First try to find any error message containing our expected text
                bool foundError = false;

                // Check for error in validation summary
                try
                {
                    IWebElement validationSummary = driver.FindElement(By.CssSelector(".validation-summary-errors"));
                    string summaryText = validationSummary.Text;
                    Console.WriteLine($"Thông báo lỗi tổng hợp: {summaryText}");

                    if (summaryText.Contains(expectedErrorMessage))
                    {
                        Console.WriteLine($"✓ Tìm thấy '{expectedErrorMessage}' trong thông báo tổng hợp");
                        foundError = true;
                    }
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("Không tìm thấy thông báo lỗi tổng hợp");
                }

                // Add more possible selectors for field errors
                List<string> selectors = new List<string>
                {
                    $"span[data-valmsg-for='{errorFieldId}']",
                    $".field-validation-error[data-valmsg-for='{errorFieldId}']",
                    $"#{errorFieldId}-error",
                    $"label[for='{errorFieldId}'] + .text-danger",
                    $"#{errorFieldId} ~ .text-danger",
                    $".error-message[data-field='{errorFieldId}']",
                    $"#error-{errorFieldId}",
                    $"[aria-describedby='{errorFieldId}-error']",
                    // Some apps append validation messages after the input field
                    $"#{errorFieldId} + span.text-danger",
                    $"#{errorFieldId} + div.text-danger",
                    // Some apps place validation messages in specific containers
                    $".form-group:has(#{errorFieldId}) .text-danger",
                    $".form-group:has(#{errorFieldId}) .error-message"
                };

                // Try each selector
                foreach (string selector in selectors)
                {
                    try
                    {
                        var errorElements = driver.FindElements(By.CssSelector(selector));
                        foreach (var errorElement in errorElements)
                        {
                            string errorText = errorElement.Text;
                            Console.WriteLine($"Tìm thấy phần tử lỗi với bộ chọn '{selector}': {errorText}");

                            if (errorText.Contains(expectedErrorMessage))
                            {
                                Console.WriteLine($"✓ Tìm thấy '{expectedErrorMessage}' trong phần tử lỗi");
                                foundError = true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Không tìm thấy phần tử lỗi với bộ chọn '{selector}'");
                    }
                }

                // Look for ANY error message on the page
                var allErrorElements = driver.FindElements(By.CssSelector(".field-validation-error, .text-danger, .validation-message, .error-message, .form-error, .invalid-feedback"));
                Console.WriteLine($"Tìm thấy {allErrorElements.Count} phần tử lỗi trên trang");

                foreach (var errorElement in allErrorElements)
                {
                    try
                    {
                        string errorText = errorElement.Text;
                        Console.WriteLine($"Thông báo lỗi: {errorText}");

                        if (errorText.Contains(expectedErrorMessage))
                        {
                            Console.WriteLine($"✓ Tìm thấy '{expectedErrorMessage}' trong phần tử lỗi");
                            foundError = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Không thể đọc nội dung phần tử lỗi: {ex.Message}");
                    }
                }

                // Check page source as a last resort
                string pageSource = driver.PageSource;
                if (pageSource.Contains(expectedErrorMessage))
                {
                    Console.WriteLine($"✓ Tìm thấy '{expectedErrorMessage}' trong mã nguồn HTML");
                    foundError = true;
                }

                // Final assertion
                Assert.That(foundError, Is.True, $"Không tìm thấy thông báo lỗi '{expectedErrorMessage}' cho trường {errorFieldId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi không mong muốn: {ex.Message}");
                Assert.Fail($"Lỗi không mong muốn khi kiểm tra lỗi '{expectedErrorMessage}' cho trường {errorFieldId}: {ex.Message}");
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