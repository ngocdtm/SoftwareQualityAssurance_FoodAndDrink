using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Interactions;
using System.Web.WebPages;
using System.Drawing;
namespace SoftwareQualityAssurance_FoodAndDrink;

public class IntergationStatusOrder
{
    private IWebDriver driver;
    private WebDriverWait wait;

    private const string MAIN_URL = "https://localhost:44379/";
    private const string HOME_URL = "https://localhost:44379/trang-chu";
    private const string PRODUCT_URL = "https://localhost:44379/san-pham";
    private const string CART_URL = "https://localhost:44379/gio-hang";
    private const string PAYMENT_URL = "https://localhost:44379/thanh-toan";
    private const string HISTORY_URL = "https://localhost:44379/review/lichsudonhang";
    private const string ORDER_SUCCESS_URL = "https://localhost:44379/ShoppingCart/CheckOutSuccess";
    private const string ADMIN_LOGIN_URL = "https://localhost:44379/Admin/Account/Login";
    private const string ADMIN_ORDER_URL = "https://localhost:44379/Admin/Order";
    private const string CUAHANG_URL = "https://localhost:44379/cua-hang";
    private const string CHINHANH7_URL = "https://localhost:44379/Products/Index?storeId=7";
    private  string AD_STATUS = "";

    public class Account 
    {
        public string Email {  get; set; } 
        public string Password { get; set; }

    }

   
    [SetUp]
    public void Setup()
    {

        driver = new ChromeDriver();
        driver.Manage().Window.Maximize();
        wait=new WebDriverWait(driver, TimeSpan.FromSeconds(100));
    }
    //User infor
    Account user = new Account
    {
        Email = "nghkphung95@gmail.com",
        Password = "newPassword#123"
    };
    //Admin Account
    Account admin = new Account
    {
        Email = "admin",
        Password = "Admin*123"
    };
    public void User_Order()
    {
        NavigateToUrl(MAIN_URL);
        LoginForUser(user.Email,user.Password);
        wait.Until(d=> d.Url == HOME_URL);  
        CreateOrder();
        Logout();

    }
    public void Admin_Order()
    {
        NavigateToUrl(ADMIN_LOGIN_URL);    
        LoginForAdmin(admin.Email,admin.Password);
        IWebElement xemDH = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[@href='/Admin/Order']")));
        xemDH.Click();
        wait.Until(d => d.Url == ADMIN_ORDER_URL);
        IWebElement btn_capnhat = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//tr[1]/td[10]/a[2]")));
        btn_capnhat.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("modal-content")));
        IWebElement statusField = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='status']")));
        statusField.Click();
        //Set trạng thái đơn= option 2: "Đơn hàng đã được duyệt"
        IWebElement option2 = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='status']/option[2]")));
        option2.Click();
        IWebElement btn_update = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[@class='btn btn-primary' and text()='Lưu cập nhật']")));
        btn_update.Click();

        IWebElement status = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//tr[1]/td[9]")));
        if (!string.IsNullOrEmpty(status.Text))
        {
            TestContext.WriteLine("Admin have updated status order");
            AD_STATUS = status.Text;
        }
        else
        {
            TestContext.WriteLine("Admin haven't updated status order");
        }
      
    }

   

    private void Logout()
    {
        IWebElement dropdown = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("dropdown")));

        Actions action = new Actions(driver);
        action.MoveToElement(dropdown).Perform();
        IWebElement btn_logout = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[./i[@class='fa fa-sign-out'] and text()=' Đăng xuất']")));
        btn_logout.Click();
    }
    private void ScrollToView(IWebElement element)
    {
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);

    }

    private void CreateOrder()
    {
        IWebElement sanpham = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[href='/san-pham']")));
        sanpham.Click();
        wait.Until(d=>d.Url==PRODUCT_URL);
        IWebElement btn_chonCuaHang = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("open-store-modal")));
        btn_chonCuaHang.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("modal-dialog")));
       wait.Until(ExpectedConditions.ElementIsVisible(By.Id("store-list")));
        IWebElement chiNhanh_CaoThang = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[@class='select-store' and @data-store-id='7']")));
        chiNhanh_CaoThang.Click();
        wait.Until(d => d.Url == CHINHANH7_URL);
        // Locate the default image element by its attributes (e.g., class or src)
        System.Threading.Thread.Sleep(2000);
        IWebElement addToCartLink = driver.FindElement(By.XPath("//a[@class='btnAddToCart' and @data-id='3' and @data-storeid='7']")); 
        addToCartLink.Click();
        IAlert alert = wait.Until(d => driver.SwitchTo().Alert());
        alert.Accept();
        IWebElement btn_cart = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.checkout a")));
        btn_cart.Click();
        wait.Until(d => d.Url == CART_URL);
        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//th[@colspan='11' and text()='Không có sản phẩm trong giỏ hàng!!']")));
        IWebElement btn_thanhtoan = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[@href='/thanh-toan' and @class='btn btn-success']")));
        btn_thanhtoan.Click();
        wait.Until(d => d.Url == PAYMENT_URL);
        //nhap ho ten
        fillOrderInfor();
       
        ////Tim trang thai don
        ////1- Chờ xác nhận
        FindStatusOrder("Chờ xác nhận");





    }

    private void fillOrderInfor()
    {
        string hoten = "NGHKP", sdt = "07883112981", diachi = "38 Tran Hung Dao";
        FillText(By.XPath("//input[@name='CustomerName']"), hoten);
        FillText(By.XPath("//input[@name='Phone']"), sdt);
        FillText(By.XPath("//input[@name='Address']"), sdt);
        IWebElement btn_Checkout = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btnCheckOut")));
        ScrollToView(btn_Checkout);
        btn_Checkout.Submit();
        wait.Until(d => d.Url == ORDER_SUCCESS_URL);
    }

    private void FindStatusOrder(string text)
    {
        IWebElement dropdown = driver.FindElement(By.ClassName("dropdown"));
        Actions action = new Actions(driver);
        action.MoveToElement(dropdown).Perform();
        IWebElement history_order = driver.FindElement(By.CssSelector(".dropdown-content a[href='/review/lichsudonhang']"));
        history_order.Click();
        wait.Until(d => d.Url == HISTORY_URL);
        Thread.Sleep(1000);

        IWebElement status = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//tr[1]/td[6]")));
        if (status.Text.Contains(text))
        {
           TestContext.WriteLine($"Order is {text}");
        }
        else
        {
            TestContext.WriteLine(status.Text);
        }

    }

    private void LoginForUser(string userMail, string userPassword)
    {
        //drop down
        IWebElement dropdown = driver.FindElement(By.ClassName("dropdown"));
        Actions action = new Actions(driver);
        action.MoveToElement(dropdown).Perform();
        IWebElement loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[href='/account/login']")));
        loginButton.Click();
        FillText(By.XPath("//*[@id='UserName']"),userMail );
        FillText(By.XPath("//*[@id='Password']"), userPassword);

      //click Dang Nhap
        IWebElement btn_Submit = wait.Until(ExpectedConditions.ElementToBeClickable(driver.FindElement(By.XPath("//*[@id='loginForm']/form/div[4]/div/input"))));
        ScrollToView(btn_Submit);
        btn_Submit.Click();
    }

    private void LoginForAdmin(string adminName, string adminPass)
    {
        FillText(By.Id("UserName"), adminName);
        FillText(By.Id("Password"),adminPass);
       IWebElement btn_login = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[text()='Đăng nhập']")));
        btn_login.Submit();
        wait.Until(d => d.Url != ADMIN_LOGIN_URL);
    }


    //General
    private void NavigateToUrl(string home_URL)
    {
        driver.Navigate().GoToUrl(home_URL);
    }


    public void FillText(By location, string text)
    {
        var element = wait.Until(ExpectedConditions.ElementIsVisible(location));
        element.Clear();
        element.SendKeys(text);
        Thread.Sleep(1000);
    }


    [Test]
    public void Test()
    {
       User_Order();
       Admin_Order();
       //tra lai user
       NavigateToUrl(HOME_URL);
       Logout();   //thoat tk admin
       LoginForUser(user.Email, user.Password);
       FindStatusOrder(AD_STATUS);

    }
    [TearDown]
    public void TearDown()
    {
        driver.Dispose();
    }
}
