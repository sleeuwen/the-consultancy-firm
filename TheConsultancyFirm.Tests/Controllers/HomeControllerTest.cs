using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TheConsultancyFirm.Controllers;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using Xunit;

namespace TheConsultancyFirm.Tests.Controllers
{
    public class HomeControllerTest
    {
        private Mock<ICustomerRepository> _customerRepository;
        public HomeControllerTest()
        {
            _customerRepository = new Mock<ICustomerRepository>();
        }
         
        [Fact]
        public async Task Index()
        {
            var model = new List<Customer>();

            _customerRepository.Setup(repo => repo.GetAll()).Returns(Task.FromResult<List<Customer>>(model));

            var controller = new HomeController(_customerRepository.Object);
            var result = await controller.Index();

            Assert.IsType<ViewResult>(result);
        }
    }
}
