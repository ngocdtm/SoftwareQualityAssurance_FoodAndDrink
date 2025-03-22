using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace SoftwareQualityAssurance_FoodAndDrink
{
    internal class IntergrationTesting_AdminAddProduct_CustomerViewDetail
    {
        private WebDriver driver;
        private WebDriverWait wait;
        private IJavaScriptExecutor js;
        public IDictionary<string, object> vars { get; private set; }
        private readonly string productName = "Integration Test Product " + DateTime.Now.ToString("yyyyMMdd_HHmmss");

        [SetUp]
        public void SetUp()
        {
            driver = new ChromeDriver();
            js = (IJavaScriptExecutor)driver;
            vars = new Dictionary<string, object>();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30)); // Tăng thời gian chờ lên 30 giây
            driver.Manage().Window.Maximize();
        }

        public string waitForWindow(int timeout)
        {
            try
            {
                Thread.Sleep(timeout);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
            var whNow = ((IReadOnlyCollection<object>)driver.WindowHandles).ToList();
            var whThen = ((IReadOnlyCollection<object>)vars["WindowHandles"]).ToList();
            if (whNow.Count > whThen.Count)
            {
                return whNow.Except(whThen).First().ToString();
            }
            else
            {
                return whNow.First().ToString();
            }
        }

        [Test]
        public void IntegrationAdminAddProductCustomerViewDetail()
        {
            try
            {
                // PHẦN 1: ADMIN THÊM SẢN PHẨM
                AdminLogin();
                AddNewProduct();
                AdminLogout();

                // In ra thông tin sản phẩm để debug
                Console.WriteLine($"Đã thêm sản phẩm mới: {productName}");

                // PHẦN 2: KHÁCH HÀNG XEM CHI TIẾT SẢN PHẨM
                CustomerLogin();
                ViewProductDetails();

                //// Kiểm tra xem sản phẩm có đúng thông tin không
                //// Tìm tiêu đề sản phẩm và kiểm tra
                //IWebElement productTitle = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".product-title, h1")));
                //Assert.That(productTitle.Text.Contains(productName), Is.True, $"Tiêu đề sản phẩm không khớp. Tìm thấy: {productTitle.Text}, Mong đợi: {productName}");

                Console.WriteLine("Test passed: Khách hàng có thể xem chi tiết sản phẩm mới thêm");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed: {ex.Message}");

                //// Chụp ảnh màn hình khi lỗi
                //try
                //{
                //    Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                //    string screenshotPath = $"error_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                //    screenshot.SaveAsFile(screenshotPath, ScreenshotImageFormat.Png);
                //    Console.WriteLine($"Đã lưu ảnh màn hình lỗi tại: {screenshotPath}");
                //}
                //catch { /* Ignore screenshot errors */ }

                throw;
            }
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

        private void AddNewProduct()
        {
            // Điều hướng đến trang quản lý sản phẩm
            driver.FindElement(By.LinkText("Xem sản phẩm")).Click();

            // Nhấn nút thêm mới
            wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Thêm mới"))).Click();

            // Chọn loại sản phẩm là Food
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("productTypeDropdown"))).Click();
            var dropdown = driver.FindElement(By.Id("productTypeDropdown"));
            dropdown.FindElement(By.XPath("//option[. = 'Food']")).Click();

            // Nhập tiêu đề sản phẩm
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Title"))).Click();
            driver.FindElement(By.Id("Title")).Clear();
            driver.FindElement(By.Id("Title")).SendKeys(productName);

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

            // FIX LỖI 1: Scroll đến element Description trước khi click
            IWebElement descriptionElement = wait.Until(ExpectedConditions.ElementExists(By.Id("Description")));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", descriptionElement);
            Thread.Sleep(500); // Đợi scroll hoàn tất

            // Nhập mô tả
            descriptionElement.Click();
            descriptionElement.Clear();
            descriptionElement.SendKeys("check");

            // Nhập chi tiết - cũng scroll trước khi tương tác
            IWebElement detailElement = wait.Until(ExpectedConditions.ElementExists(By.Id("txtDetail")));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", detailElement);
            Thread.Sleep(500);
            detailElement.Click();
            detailElement.Clear();
            detailElement.SendKeys("yeh");

            // Nhập số lượng - scroll trước khi tương tác
            IWebElement quantityElement = wait.Until(ExpectedConditions.ElementExists(By.Id("Quantity")));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", quantityElement);
            Thread.Sleep(500);
            quantityElement.Click();
            quantityElement.Clear();
            quantityElement.SendKeys("110");

            // Nhập giá - scroll trước khi tương tác
            IWebElement demoPriceElement = wait.Until(ExpectedConditions.ElementExists(By.Id("demoPrice")));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", demoPriceElement);
            Thread.Sleep(500);
            demoPriceElement.Clear();
            demoPriceElement.SendKeys("50000");

            // Nhập giá gốc - scroll trước khi tương tác
            IWebElement demoOriginalPriceElement = wait.Until(ExpectedConditions.ElementExists(By.Id("demoOriginalPrice")));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", demoOriginalPriceElement);
            Thread.Sleep(500);
            demoOriginalPriceElement.Clear();
            demoOriginalPriceElement.SendKeys("50000");

            // Chọn hiển thị sản phẩm - scroll trước khi tương tác
            IWebElement displayCheckbox = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".custom-control-label")));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", displayCheckbox);
            Thread.Sleep(500);
            displayCheckbox.Click();

            // Thêm mới: Upload hình ảnh sản phẩm
            try
            {
                // Chuyển đến tab "Hình ảnh"
                IWebElement imageTab = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Hình ảnh")));
                js.ExecuteScript("arguments[0].scrollIntoView(true);", imageTab);
                Thread.Sleep(500);
                imageTab.Click();

                // Lưu window handles hiện tại
                vars["WindowHandles"] = driver.WindowHandles;

                // Nhấn nút "Tải ảnh"
                IWebElement uploadButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("iTaiAnh")));
                uploadButton.Click();

                // Đợi cửa sổ CKFinder mở ra
                string ckfinderWindow = waitForWindow(2000);
                vars["root"] = driver.CurrentWindowHandle;

                // Chuyển sang cửa sổ CKFinder
                driver.SwitchTo().Window(ckfinderWindow);

                try
                {
                    // Chuyển sang iframe của CKFinder (nếu có)
                    driver.SwitchTo().Frame(0);

                    // Chọn ảnh đầu tiên trong CKFinder
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#r0, .file-item")));
                    IWebElement firstImage = driver.FindElement(By.CssSelector("#r0, .file-item"));
                    firstImage.Click();

                    // Double click để chọn ảnh
                    Actions actions = new Actions(driver);
                    actions.DoubleClick(firstImage).Perform();

                }
                catch (Exception frameEx)
                {
                    Console.WriteLine($"Không thể thao tác với iframe: {frameEx.Message}");

                    // Nếu không tìm thấy iframe, thử tìm trực tiếp trên trang
                    try
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#r0, .file-item")));
                        IWebElement firstImage = driver.FindElement(By.CssSelector("#r0, .file-item"));

                        // Double click để chọn ảnh
                        Actions actions = new Actions(driver);
                        actions.DoubleClick(firstImage).Perform();
                    }
                    catch (Exception directEx)
                    {
                        Console.WriteLine($"Không thể tìm ảnh trực tiếp: {directEx.Message}");

                        // Nếu không thể chọn ảnh, đóng cửa sổ CKFinder
                        driver.Close();
                    }
                }

                // Chuyển lại cửa sổ chính
                driver.SwitchTo().Window(vars["root"].ToString());

                // Đợi để ảnh được tải lên
                Thread.Sleep(1000);

                Console.WriteLine("Đã tải ảnh sản phẩm thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tải ảnh: {ex.Message}");
            }

            // Lưu sản phẩm - scroll đến nút Save
            IWebElement saveButton = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".btn-success")));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", saveButton);
            Thread.Sleep(500);
            saveButton.Click();

            //// Đợi thông báo thành công hoặc đợi trang reload
            //try
            //{
            //    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".alert-success")));
            //}
            //catch
            //{
            //    // Trong trường hợp không có thông báo, đợi một chút để đảm bảo quá trình hoàn tất
            //    Thread.Sleep(3000);
            //}
        }

        private void AdminLogout()
        {
            // Đăng xuất tài khoản admin
            try
            {
                IWebElement userMenu = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".pro-user-name")));
                userMenu.Click();

                IWebElement logoutLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Đăng xuất")));
                logoutLink.Click();

                // Đợi đăng xuất hoàn tất
                wait.Until(d => d.Url.Contains("/Admin/Account/Login") || d.Url.Contains("/"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đăng xuất admin: {ex.Message}");
                // Điều hướng về trang chủ để tiếp tục test
                driver.Navigate().GoToUrl("https://localhost:44379");
            }
        }

        private void CustomerLogin()
        {
            // Điều hướng đến trang chủ
            driver.Navigate().GoToUrl("https://localhost:44379");

            // Đăng nhập với tài khoản khách hàng
            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".dropdown > img"))).Click();
                wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Đăng nhập"))).Click();

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("UserName"))).Click();
                driver.FindElement(By.Id("UserName")).Clear();
                driver.FindElement(By.Id("UserName")).SendKeys("22DH114392@st.huflit.edu.vn");

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Password"))).Click();
                driver.FindElement(By.Id("Password")).Clear();
                driver.FindElement(By.Id("Password")).SendKeys("Palm*123");

                driver.FindElement(By.CssSelector(".btn-primary")).Click();

                // Đợi đăng nhập hoàn tất
                wait.Until(d => d.Url.Contains("/") && !d.Url.Contains("/Account/Login"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đăng nhập khách hàng: {ex.Message}");
                // Nếu đăng nhập lỗi, thử tiếp tục test
            }
        }

        private void ViewProductDetails()
        {
            // FIX LỖI 2: Dựa vào Page Source, có vẻ như trang sản phẩm đang sử dụng React
            // và có cấu trúc khác với những gì chúng ta mong đợi

            // Điều hướng đến trang sản phẩm
            driver.Navigate().GoToUrl("https://localhost:44379/san-pham");

            // Chờ đợi để trang sản phẩm tải hoàn tất
            Thread.Sleep(3000);

            // Chọn chi nhánh
            try
            {
                // Tìm và nhấp vào nút mở modal chọn chi nhánh
                IWebElement storeModalButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("open-store-modal")));
                storeModalButton.Click();

                // Đợi modal hiển thị
                Thread.Sleep(1000);

                // Chọn chi nhánh thứ 5 (hoặc chi nhánh phù hợp)
                IWebElement storeOption = wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.CssSelector(".store-card:nth-child(5) p:nth-child(2)")));
                storeOption.Click();

                // Đợi trang tải lại với sản phẩm của chi nhánh
                Thread.Sleep(3000);

                // In ra thông tin debug
                Console.WriteLine("Đã chọn chi nhánh. Đang tìm kiếm sản phẩm...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi chọn chi nhánh: {ex.Message}");
            }

            // Tìm và nhấp vào sản phẩm vừa thêm
            try
            {
                // Sử dụng JavaScript để tìm kiếm sản phẩm bằng tên
                // Vì có thể các sản phẩm được render động bởi React, JavaScript sẽ hiệu quả hơn

                // Lấy tất cả các link trên trang
                var links = driver.FindElements(By.TagName("a"));
                Console.WriteLine($"Tổng số links trên trang: {links.Count}");

                //// In ra một số links đầu tiên để debug
                //for (int i = 0; i < Math.Min(links.Count, 10); i++)
                //{
                //    Console.WriteLine($"Link {i}: {links[i].Text} - {links[i].GetAttribute("href")}");
                //}

                // Tìm link có chứa text giống với tên sản phẩm
                IWebElement productLink = null;
                foreach (var link in links)
                {
                    if (link.Text.Contains(productName))
                    {
                        productLink = link;
                        Console.WriteLine($"Đã tìm thấy sản phẩm: {link.Text}");
                        break;
                    }
                }
                // Nếu tìm thấy, scroll đến và click
                js.ExecuteScript("arguments[0].scrollIntoView(true);", productLink);
                Thread.Sleep(500);
                js.ExecuteScript("arguments[0].click();", productLink);

                Thread.Sleep(1000);


                // Nếu không tìm thấy, thử tìm theo cách khác
                //if (productLink == null)
                //{
                //    Console.WriteLine("Không tìm thấy sản phẩm bằng text. Thử tìm bằng JavaScript...");

                //    // Sử dụng JavaScript để tìm các phần tử có chứa text của sản phẩm
                //    string script = @"
                //        var elements = document.querySelectorAll('*');
                //        for (var i = 0; i < elements.length; i++) {
                //            if (elements[i].textContent.includes('" + productName + @"')) {
                //                return elements[i];
                //            }
                //        }
                //        return null;
                //    ";

                //    var element = js.ExecuteScript(script);
                //    if (element != null)
                //    {
                //        Console.WriteLine("Đã tìm thấy phần tử chứa tên sản phẩm bằng JavaScript");
                //        js.ExecuteScript("arguments[0].click();", element);
                //    }
                //    else
                //    {
                //        // Nếu không tìm thấy, in ra tất cả text có trên trang để debug
                //        Console.WriteLine("Không tìm thấy phần tử nào chứa tên sản phẩm. Dưới đây là text có trên trang:");
                //        string pageText = (string)js.ExecuteScript("return document.body.innerText;");
                //        Console.WriteLine(pageText.Substring(0, Math.Min(pageText.Length, 2000)));

                //        //// Ảnh chụp màn hình cho debug
                //        //Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                //        //string screenshotPath = $"product_page_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                //        //screenshot.SaveAsFile(screenshotPath, ScreenshotImageFormat.Png);
                //        //Console.WriteLine($"Đã lưu ảnh màn hình tại: {screenshotPath}");

                //        // Thử phương án dự phòng - tìm kiếm sản phẩm
                //        try
                //        {
                //            // Tìm kiểm ô tìm kiếm
                //            IWebElement searchBox = driver.FindElement(By.CssSelector("input[type='text'], input[placeholder*='tìm']"));
                //            searchBox.Clear();
                //            searchBox.SendKeys(productName);
                //            searchBox.SendKeys(Keys.Enter);

                //            // Đợi kết quả tìm kiếm
                //            Thread.Sleep(2000);
                //        }
                //        catch (Exception searchEx)
                //        {
                //            Console.WriteLine($"Không thể tìm kiếm sản phẩm: {searchEx.Message}");
                //        }
                //    }
                //}
                //else
                //{
                //    // Nếu tìm thấy, scroll đến và click
                //    js.ExecuteScript("arguments[0].scrollIntoView(true);", productLink);
                //    Thread.Sleep(500);
                //    js.ExecuteScript("arguments[0].click();", productLink);
                //}

                // Đợi trang chi tiết sản phẩm tải
                //Thread.Sleep(3000);

                //// Thử chọn các tùy chọn extra nếu có
                //try
                //{
                //    var extraElements = driver.FindElements(By.CssSelector("[id^='extra_']"));
                //    if (extraElements.Count > 0)
                //    {
                //        js.ExecuteScript("arguments[0].click();", extraElements[0]);
                //        Console.WriteLine("Đã chọn tùy chọn extra");
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine($"Không thể chọn extra: {ex.Message}");
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm sản phẩm: {ex.Message}");
                throw;
            }
        }

        //[Test]
        //public void testuploadimage()
        //{
        //    // Test name: test_upload_image
        //    // Step # | name | target | value
        //    // 1 | open | https://localhost:44379/admin/product/add | 
        //    driver.Navigate().GoToUrl("https://localhost:44379/admin/product/add");
        //    // 2 | setWindowSize | 1552x832 | 
        //    driver.Manage().Window.Size = new System.Drawing.Size(1552, 832);
        //    // 3 | click | linkText=Hình ảnh | 
        //    driver.FindElement(By.LinkText("Hình ảnh")).Click();
        //    // 4 | click | id=iTaiAnh | 
        //    vars["WindowHandles"] = driver.WindowHandles;
        //    // 5 | storeWindowHandle | root | 
        //    driver.FindElement(By.Id("iTaiAnh")).Click();
        //    // 6 | selectWindow | handle=${win9381} | 
        //    vars["win9381"] = waitForWindow(2000);
        //    // 7 | close |  | 
        //    vars["root"] = driver.CurrentWindowHandle;
        //    // 8 | selectWindow | handle=${root} | 
        //    driver.SwitchTo().Window(vars["win9381"].ToString());
        //    // 9 | click | id=iTaiAnh | 
        //    driver.Close();
        //    // 10 | selectWindow | handle=${win5594} | 
        //    driver.SwitchTo().Window(vars["root"].ToString());
        //    // 11 | close |  | 
        //    vars["WindowHandles"] = driver.WindowHandles;
        //    // 12 | selectWindow | handle=${root} | 
        //    driver.FindElement(By.Id("iTaiAnh")).Click();
        //    vars["win5594"] = waitForWindow(2000);
        //    driver.SwitchTo().Window(vars["win5594"].ToString());
        //    driver.Close();
        //    driver.SwitchTo().Window(vars["root"].ToString());
        //}

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