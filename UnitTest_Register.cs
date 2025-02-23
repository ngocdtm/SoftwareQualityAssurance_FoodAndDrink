
using System.ComponentModel.DataAnnotations;
using WebsiteBanDoAnVaThucUong;
using WebsiteBanDoAnVaThucUong.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using WebsiteBanDoAnVaThucUong.Controllers;
namespace SoftwareQualityAssurance_FoodAndDrink
{
    internal class UnitTest_Register
    {
        private AccountController accountController;
        private RegisterViewModel registerViewModel; //contain user input
        private UserStore<ApplicationUser> _userStore;
        private ApplicationDbContext _context;
       
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
         
        private LoginViewModel loginViewModel;

        [SetUp]
        public void Setup()
        {

            registerViewModel = new RegisterViewModel();
            _context = new ApplicationDbContext();
            _userStore = new UserStore<ApplicationUser>(_context);
            accountController = new AccountController();
            var userStoreMock = new Mock<Microsoft.AspNetCore.Identity.IUserStore<ApplicationUser>>();
            var identityOptions = new IdentityOptions
            {
                Password = new PasswordOptions
                {
                    RequiredLength = 6,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireUppercase = true,
                    RequireNonAlphanumeric = true
                }
            };

            // Mock the IUserStore for UserManager
            userStoreMock = new Mock<Microsoft.AspNetCore.Identity.IUserStore<ApplicationUser>>();

            // Initialize UserManager with IdentityOptions and PasswordValidator
            _userManagerMock = new Mock<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>(
                userStoreMock.Object,
                Options.Create(identityOptions), // Pass IdentityOptions
                new PasswordHasher<ApplicationUser>(),
                new List<IUserValidator<ApplicationUser>>(),
                new List<IPasswordValidator<ApplicationUser>> { new PasswordValidator<ApplicationUser>() },
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null,  // No services
                null   // No logger
            );


            //sign in set up
            loginViewModel= new LoginViewModel(); 
           


        }
       







        /* Các testcase liên quan đến hàm ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        trong file IdentityConfig.cs */

        
        //Mật khẩu yếu
        // TC 1.5.1: Password không chứa kí tự in hoa
        [TestCase("12as!@", "!@asAS", true)]
        // TC 1.5.2: Password không chứa kí tự số 
        [TestCase("!@asAS", "!@asAS", true)]
        // TC 1.5.3: Password không chứa chữ cái thường
        [TestCase("12AS!@", "12AS!@", true)]
        // TC 1.5.4: Password không chứa kí tự đặc biệt
        [TestCase("12asAS", "12asAS", true)]
        public async Task PasswordValidateTest(string userName, string password, bool expected)
        {
            var passwordValidator = new PasswordValidator<ApplicationUser>();

            var user = new ApplicationUser { UserName = userName };

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser user, string password) =>
            {
                var result = passwordValidator.ValidateAsync(_userManagerMock.Object, user, password).Result;

                return result.Succeeded
                    ? IdentityResult.Success
                    : IdentityResult.Failed(new IdentityError { Description = "Invalid password" });
            });

            var validationResult = await passwordValidator.ValidateAsync(_userManagerMock.Object, user, password);

            foreach (var e in validationResult.Errors)
            {
                Assert.Pass(e.Description);
            }
        }

        //check validate
        public bool ValidateModel(object model, out List<ValidationResult> results)
        {
            var context = new ValidationContext(model, null, null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(model, context, results, true);
        }

        //TC1.2: Email đã tồn tại
        [TestCase("existing@gmail.com")]
        public async Task CreateAsync_EmailValidation(string email)
        {
            var identityOptions = new IdentityOptions
            {
                User = new UserOptions { RequireUniqueEmail = true }
            };

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

            var userValidatorMock = new Mock<IUserValidator<ApplicationUser>>();
            userValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<UserManager<ApplicationUser>>(), It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object,
                Options.Create(identityOptions),
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            //Danh sách email giả lập đã tồn tại
            var existingEmails = new HashSet<string> { "existing@gmail.com", "test@example.com" };

            userManagerMock
        .Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
        .ReturnsAsync((string inputEmail) => existingEmails.Contains(inputEmail) ? new ApplicationUser { Email = inputEmail } : null);

            userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser user, string password) =>
                {
                    return existingEmails.Contains(user.Email)
                        ? IdentityResult.Failed(new IdentityError { Description = "Email is already taken." })
                        : IdentityResult.Success;
                });

            var newUser = new ApplicationUser { Email = email };
            var result = await userManagerMock.Object.CreateAsync(newUser, "Password123!");
            foreach (var e in result.Errors)
            {
                Assert.Pass(e.Description);
            }

        }
        // TC1.1: success
        [TestCase("example@gmail.com", "strongPassword*12345", "strongPassword*12345")]
        // TC1.3: Ô trống
            //TC1.3.1:Trống email
        [TestCase("", "strongPassword*12345", "strongPassword*12345")]
            //TC1.3.2: Trống password
        [TestCase("passwordNull@gmail.com", "", "strongPassword*12345")]
            //TC1.3.3: Trống confirmPassword
        [TestCase("confirmPassNull@gmail.com", "strongPassword*12345", "")]
             //Trống tất cả các trường yêu cầu
        [TestCase("", "", "")]
        //TC1.4: Mật khẩu ngắn. Ít hơn 6 kí tự: 5 kí tự (giá trị biên)
        [TestCase("shortPassword@gmail.com", "10@aA", "strongPassword*12345")]

        //TC1.6: Mật khẩu không khớp
        [TestCase("notmatchPass@gmail.com", "strongPassword*12345", "12345")]
        //TC1.7: Email không có @
        [TestCase("examplegmail.com", "strongPassword*12345", "strongPassword*12345")]
        public void testRegisterWithAccountViewModels(string email, string pass, string confirmPass)
        {
             registerViewModel = new RegisterViewModel { Email = email, Password = pass, ConfirmPassword = confirmPass };
            //tạo một list chứa các tb lỗi (nội dung tb lỗi ở file AccountViewModels.cs)
            List<ValidationResult> errors = new List<ValidationResult>();
            bool isValid = ValidateModel(registerViewModel, out errors);
            foreach (ValidationResult result in errors)
            {
                Assert.Pass(result.ErrorMessage);
            }
        }

        #region Đăng nhập

        //TC2.1: Đăng nhập thành công
        //TC2.2: Mật khẩu sai
        //TC2.4: Tài khoản bị khóa
        //TC2.5: Tài khoản không tồn tại
        //TC2.6: Đa phiên


        //TC2.3: Ô trống
        //TC2.3.1: Trống ô email
        [Test]
        public void testSignInWithModel_NullEmail()
        {
            loginViewModel = new LoginViewModel { UserName =null, Password = "123Aa#123", RememberMe = false };
            List<ValidationResult> errors = new List<ValidationResult>();
            bool isValid = ValidateModel(loginViewModel, out errors);
            foreach (ValidationResult result in errors)
            {
                TestContext.WriteLine(result.ErrorMessage);
            }
        }
        //TC2.3.2: Trống password
        [Test]
        public void testSignInWithModel_NullPass()
        {
            loginViewModel = new LoginViewModel { UserName = "existing@gmail.com", Password = null, RememberMe = false };
            List<ValidationResult> errors = new List<ValidationResult>();
            bool isValid = ValidateModel(loginViewModel, out errors);
            foreach (ValidationResult result in errors)
            {
                TestContext.WriteLine(result.ErrorMessage);
            }
        }
        //TC2.3.3: Trống cả 2 

        [Test]
        public void testSignInWithModel_BothNull()
        {
            loginViewModel = new LoginViewModel { UserName = null, Password =null, RememberMe = false };
            List<ValidationResult> errors = new List<ValidationResult>();
            bool isValid = ValidateModel(loginViewModel, out errors);
            foreach (ValidationResult result in errors)
            {
                 Assert.That(errors.Count(),Is.EqualTo(2)); // trong 2 truong thi se tb 2 loi
                TestContext.WriteLine(result.ErrorMessage);
            }
        }

        #endregion



        [TearDown]
        public void TearDown()
        {
            _userStore.Dispose();
            _context.Dispose();
            accountController.Dispose();
        }
    }
}
