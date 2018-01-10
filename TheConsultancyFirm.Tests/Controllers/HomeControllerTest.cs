using System;
using System.Collections.Generic;
using System.Linq;
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
        private Mock<INewsItemRepository> _newsItemRepository;

        public HomeControllerTest()
        {
            _customerRepository = new Mock<ICustomerRepository>();
            _newsItemRepository = new Mock<INewsItemRepository>();
        }
         
        [Fact]
        public async Task Index()
        {
            var model = new List<Customer>();
            var model2 = new List<NewsItem>
            {
                new NewsItem
                {
                    Id = 0,
                    LastModified = DateTime.UtcNow,
                    Title = "Title 1"
                }
            }.AsQueryable();

            _customerRepository.Setup(repo => repo.GetAll()).Returns(Task.FromResult<List<Customer>>(model));
            _newsItemRepository.Setup(repo => repo.GetAll()).Returns(model2);

            var controller = new HomeController(_customerRepository.Object, _newsItemRepository.Object);
            var result = await controller.Index();

            Assert.IsType<ViewResult>(result);
        }
    }
}
