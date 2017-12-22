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
        private readonly Mock<IContactRepository> _contactRepository;
        public ContactsControllerTest()
        {
            _contactRepository = new Mock<IContactRepository>();
        }

        [Fact]
        public async Task Index()
        {
            var controller = new ContactsController(_contactRepository.Object);
            var result = await controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
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
                Readed = false
            };

            _contactRepository.Setup(repo => repo.Get(0)).Returns(Task.FromResult<Contact>(model));
            
            var controller = new ContactsController(_contactRepository.Object);

            var result = await controller.Details(model.Id);

            var viewResult = Assert.IsType<ViewResult>(result);
            model.Readed = true;
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
                Readed = false
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
                Readed = true
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
                Readed = false
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
                Readed = true
            };

            _contactRepository.Setup(repo => repo.Get(0)).Returns(Task.FromResult(model));

            var controller = new ContactsController(_contactRepository.Object);

            var result = await controller.Details(model.Id);

            Assert.IsType<ViewResult>(result);

            _contactRepository.Verify(repo => repo.Update(model), Times.Never);
        }

        [Fact] public async Task Details_ReadIsFalse()
        {
            var model = new Contact
            {
                Id = 0,
                Name = "Kevin",
                Email = "kevin@example.com",
                Subject = "Anders",
                Message = "I have a question.",
                Readed = false
            };

            _contactRepository.Setup(repo => repo.Get(0)).Returns(Task.FromResult(model));

            var controller = new ContactsController(_contactRepository.Object);

            var result = await controller.Details(model.Id);

            Assert.IsType<ViewResult>(result);

            _contactRepository.Verify(repo => repo.Update(model), Times.Once);
        }

    }
}
