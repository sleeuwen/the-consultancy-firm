using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
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
        private Mock<ISolutionRepository> _solutionRepository;
        private Mock<ICaseRepository> _caseRepository;

        public HomeControllerTest()
        {
            _customerRepository = new Mock<ICustomerRepository>();
            _newsItemRepository = new Mock<INewsItemRepository>();
            _solutionRepository = new Mock<ISolutionRepository>();
            _caseRepository = new Mock<ICaseRepository>();
        }

        [Fact]
        public async Task Index()
        {
            var model = new List<Customer>();
            var model2 = new List<NewsItem>().AsQueryable().BuildMock();
            var solutions = new List<Solution>().AsQueryable().BuildMock();
            var cases = new List<Case>();

            _customerRepository.Setup(repo => repo.GetAll()).Returns(Task.FromResult<List<Customer>>(model));
            _newsItemRepository.Setup(repo => repo.GetAll()).Returns(model2.Object);
            _solutionRepository.Setup(repo => repo.GetAll()).Returns(solutions.Object);
            _caseRepository.Setup(repo => repo.GetHomepageItems()).Returns(Task.FromResult(cases));

            var controller = new HomeController(_customerRepository.Object, _newsItemRepository.Object, _solutionRepository.Object, _caseRepository.Object);
            var result = await controller.Index();

            Assert.IsType<ViewResult>(result);
        }
    }
}
