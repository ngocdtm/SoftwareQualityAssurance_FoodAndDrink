//using System.Web;
//using System.Web.Mvc;
//using Microsoft.AspNetCore.Routing;

//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.Owin;
//using Microsoft.Owin.Security;
//using Moq;
//using WebsiteBanDoAnVaThucUong.Models;
//using WebsiteBanDoAnVaThucUong;
//using WebsiteBanDoAnVaThucUong.Controllers; // Replace with your actual namespace

//[TestFixture]
//public class AccountControllerTests
//{
//    private Mock<HttpContextBase> _mockHttpContext;
//    private Mock<IOwinContext> _mockOwinContext;
//    private Mock<IAuthenticationManager> _mockAuthManager;
//    private Mock<ApplicationSignInManager> _mockSignInManager;
//    private Mock<ApplicationUserManager> _mockUserManager;

//    private AccountController _accountController;

//    [SetUp]
//    public void SetUp()
//    {
//        // Create mock HttpContext and OwinContext
//        _mockHttpContext = new Mock<HttpContextBase>();
//        _mockOwinContext = new Mock<IOwinContext>();
//        _mockAuthManager = new Mock<IAuthenticationManager>();

//        // Mock UserManager and SignInManager
//        _mockUserManager = new Mock<ApplicationUserManager>(new Mock<IUserStore<ApplicationUser>>().Object);
//        _mockSignInManager = new Mock<ApplicationSignInManager>(_mockUserManager.Object, _mockHttpContext.Object);

//        // Set up OwinContext to return mocks
//        _mockHttpContext.Setup(c => c.GetOwinContext()).Returns(_mockOwinContext.Object);
//        _mockOwinContext.Setup(o => o.Authentication).Returns(_mockAuthManager.Object);
//        _mockOwinContext.Setup(o => o.Get<ApplicationUserManager>()).Returns(_mockUserManager.Object);
//        _mockOwinContext.Setup(o => o.Get<ApplicationSignInManager>()).Returns(_mockSignInManager.Object);

//        // Set up Controller with mocked HttpContext
//        _accountController = new AccountController();
//        _accountController.ControllerContext = new ControllerContext(_mockHttpContext.Object, new RouteData(), _accountController);
//    }

//    //[Test]
//    //public async Task Login_ValidUser_RedirectsToHome()
//    //{
//    //    // Arrange
//    //    var loginModel = new LoginViewModel
//    //    {
//    //        Email = "test@example.com",
//    //        Password = "ValidPassword123",
//    //        RememberMe = false
//    //    };

//    //    _mockSignInManager
//    //        .Setup(m => m.PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RememberMe, false))
//    //        .ReturnsAsync(SignInStatus.Success);

//    //    // Act
//    //    var result = await _accountController.Login(loginModel, returnUrl: "/Home") as RedirectResult;

//    //    // Assert
//    //    Assert.IsNotNull(result);
//    //    Assert.AreEqual("/Home", result.Url);
//    //}

//    [Test]
//    public async Task Login_InvalidUser_ReturnsViewWithError()
//    {
//        // Arrange
//        var loginModel = new LoginViewModel
//        {
//            UserName = "invalid@example.com",
//            Password = "WrongPassword",
//            RememberMe = false
//        };

//        _mockSignInManager
//            .Setup(m => m.PasswordSignInAsync(loginModel.UserName, loginModel.Password, loginModel.RememberMe, false))
//            .ReturnsAsync(SignInStatus.Failure);

//        // Act
//        var result = await _accountController.Login(loginModel, returnUrl: "/Home") as ViewResult;

//        // Assert
//        Assert.That(result,Is.Not.Null);
//        Assert.That(result.ViewData.ModelState.ContainsKey(""));
//    }
//}
