using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using TheConsultancyFirm.Areas.Dashboard.Controllers;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using Xunit;

namespace TheConsultancyFirm.Tests.Controllers
{
    [Area("Dashboard")]
    public class ContactsControllerTest
    {
        [Fact]
        public async Task Index()
        {
            var contactRepository = new Mock<IContactRepository>();
            var controller = new ContactsController(contactRepository.Object);
            var actionResultTask = await controller.Index();;
            var result = actionResultTask as ViewResult;
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Details()
        {
            var controller = new ContactsController(null);

            var model = new Contact
            {
                Id = 0,
                Name = "Kevin",
                Email = "kevin@example.com",
                Subject = "Anders",
                Message = "I have a question."
            };

            var contactRepository = new Mock<IContactRepository>();
            contactRepository.Setup(repo => repo.AddAsync(model)).Returns(Task.FromResult(0));

            var result = controller.Details(model.Id);

            Assert.IsType<ViewResult>(result);
        }
    }
}
