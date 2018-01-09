using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TheConsultancyFirm.Controllers;
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
            var controller = new HomeController(_customerRepository.Object);
            var result = await controller.Index();

            Assert.IsType<ViewResult>(result);
        }
    }
}
