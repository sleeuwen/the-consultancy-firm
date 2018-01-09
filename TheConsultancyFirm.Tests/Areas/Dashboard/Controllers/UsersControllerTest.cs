using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Areas.Dashboard.Controllers;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Services;
using Xunit;

namespace TheConsultancyFirm.Tests.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class UsersControllerTest
    {
        private readonly Mock<IMailService> _mailService;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<ApplicationUser> _applicationUser;

        public UsersControllerTest()
        {
            _mailService = new Mock<IMailService>();
            _userManager = new Mock<UserManager<ApplicationUser>>(null, null, null, null, null, null, null, null, null);
            _applicationUser = new Mock<ApplicationUser>();
        }

        [Fact]
        public async Task UsersControllerIndex()
        {
            var controller = new UsersController(_userManager.Object, _mailService.Object);
            var result = await controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Valid_User()
        {
            var model = new ApplicationUser
            {
                Id = "id123",
                Email = "test@hotmail.nl",
            };

            _userManager.Setup(repo => repo.CreateAsync(model)).Returns(Task.FromResult<IdentityResult>(IdentityResult.Success));
            var controller = new UsersController(_userManager.Object, _mailService.Object);
            var actionResult = await controller.Create(model);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            _userManager.Verify(repo => repo.CreateAsync(model), Times.Once);
        }

        [Fact]
        public async Task Create_Invalid_User()
        {

        }

        [Fact]
        public async Task Email_Is_Send()
        {
            var model = new ApplicationUser
            {
                Id = "id123",
                Email = "test@hotmail.nl",
            };
            string subject = "Er is een account gecreëerd voor u op de website.";
            string message = "Dit is het bericht in de mail";

            _userManager.Setup(repo => repo.CreateAsync(model)).Returns(Task.FromResult<IdentityResult>(IdentityResult.Success));
            _mailService.Setup(repo => repo.SendMailAsync(model.Email, subject, message)).Returns(Task.FromResult(0));

            var controller = new UsersController(_userManager.Object, _mailService.Object);
            var actionResult = await controller.Create(model);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            _mailService.Verify(repo => repo.SendMailAsync(model.Email, subject, message), Times.Once);
        }

        [Fact]
        public async Task Delete_User()
        {
            var model = new ApplicationUser
            {
                Id = "id123",
                Email = "test@hotmail.nl",
                Enabled = true
            };

            var controller = new UsersController(null, null);
            var actionResult = await controller.Create(model);

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            model.Enabled = false;
            Assert.Equal(model, viewResult.Model);
        }
    }
}
