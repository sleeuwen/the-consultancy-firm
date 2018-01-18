using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TheConsultancyFirm.Areas.Dashboard.Controllers;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using Xunit;

namespace TheConsultancyFirm.Tests.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class ContactsControllerTest
    {
        private readonly Mock<IContactRepository> _contactRepository;

        public ContactsControllerTest()
        {
            _contactRepository = new Mock<IContactRepository>();
        }

        [Fact]
        public async Task Details_Success()
        {
            var model = new Contact
            {
                Id = 0,
                Name = "Kevin",
                Email = "kevin@example.com",
                Subject = "Anders",
                Message = "I have a question.",
                Read = false
            };

            _contactRepository.Setup(repo => repo.Get(0)).Returns(Task.FromResult<Contact>(model));

            var controller = new ContactsController(_contactRepository.Object);

            var result = await controller.Details(model.Id);

            var viewResult = Assert.IsType<ViewResult>(result);
            model.Read = true;
            Assert.Equal(model, viewResult.Model);

            _contactRepository.Verify(repo => repo.Update(model), Times.Once);
        }

        [Fact]
        public async Task Details_NullId()
        {
            var model = new Contact
            {
                Id = 0,
                Name = "Kevin",
                Email = "kevin@example.com",
                Subject = "Anders",
                Message = "I have a question.",
                Read = false
            };

            _contactRepository.Setup(repo => repo.Get(0)).Returns(Task.FromResult<Contact>(null));

            var controller = new ContactsController(_contactRepository.Object);

            var result = await controller.Details(null);

            Assert.IsType<NotFoundResult>(result);

            _contactRepository.Verify(repo => repo.Update(model), Times.Never);
        }

        [Fact]
        public async Task Details_ValidId()
        {
            var model = new Contact
            {
                Id = 0,
                Name = "Kevin",
                Email = "kevin@example.com",
                Subject = "Anders",
                Message = "I have a question.",
                Read = true
            };

            _contactRepository.Setup(repo => repo.Get(0)).Returns(Task.FromResult(model));

            var controller = new ContactsController(_contactRepository.Object);

            var result = await controller.Details(model.Id);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);

            _contactRepository.Verify(repo => repo.Update(model), Times.Never);
        }

        [Fact]
        public async Task Details_InvalidModel()
        {
            var model = new Contact
            {
                Id = 0,
                Name = "Kevin",
                Email = "kevin@example.com",
                Subject = "Anders",
                Message = "I have a question.",
                Read = false
            };

            _contactRepository.Setup(repo => repo.Get(0)).Returns(Task.FromResult<Contact>(null));

            var controller = new ContactsController(_contactRepository.Object);

            var result = await controller.Details(model.Id);

            Assert.IsType<NotFoundResult>(result);

            _contactRepository.Verify(repo => repo.Update(model), Times.Never);
        }

        [Fact]
        public async Task Details_ReadIsTrue()
        {
            var model = new Contact
            {
                Id = 0,
                Name = "Kevin",
                Email = "kevin@example.com",
                Subject = "Anders",
                Message = "I have a question.",
                Read = true
            };

            _contactRepository.Setup(repo => repo.Get(0)).Returns(Task.FromResult(model));

            var controller = new ContactsController(_contactRepository.Object);

            var result = await controller.Details(model.Id);

            Assert.IsType<ViewResult>(result);

            _contactRepository.Verify(repo => repo.Update(model), Times.Never);
        }

        [Fact]
        public async Task Details_ReadIsFalse()
        {
            var model = new Contact
            {
                Id = 0,
                Name = "Kevin",
                Email = "kevin@example.com",
                Subject = "Anders",
                Message = "I have a question.",
                Read = false
            };

            _contactRepository.Setup(repo => repo.Get(0)).Returns(Task.FromResult(model));

            var controller = new ContactsController(_contactRepository.Object);

            var result = await controller.Details(model.Id);

            Assert.IsType<ViewResult>(result);

            _contactRepository.Verify(repo => repo.Update(model), Times.Once);
        }
    }
}
