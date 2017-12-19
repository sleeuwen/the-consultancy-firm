using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using TheConsultancyFirm.Controllers;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;
using Xunit;

namespace TheConsultancyFirm.Tests.Controllers
{
    public class ContactControllerTest
    {
        [Fact]
        public void Index()
        {
            var controller = new ContactController(null, null);

            var result = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Contact>(viewResult.Model);
        }

        [Fact]
        public async void SendInvalidContactForm()
        {
            var controller = new ContactController(null, null);

            controller.ModelState.AddModelError("Name", "Cannot be empty");

            var result = await controller.Index(new Contact());
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void SendValidContactForm_SavesToDb()
        {
            var model = new Contact
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Subject = "Anders",
                Message = "I have a question."
            };

            var contactRepository = new Mock<IContactRepository>();
            contactRepository.Setup(repo => repo.AddAsync(model)).Returns(Task.FromResult(0));

            var mailService = new Mock<IMailService>();
            mailService.Setup(mail => mail.SendContactMailAsync(model)).Returns(Task.FromResult(0));

            var controller = new ContactController(contactRepository.Object, mailService.Object);
            controller.TempData = new Mock<ITempDataDictionary>().Object;

            var result = await controller.Index(model);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("contact", redirectToActionResult.Fragment);

            contactRepository.Verify(repo => repo.AddAsync(model), Times.Once);
            mailService.Verify(repo => repo.SendContactMailAsync(model), Times.Once);
        }
    }
}
