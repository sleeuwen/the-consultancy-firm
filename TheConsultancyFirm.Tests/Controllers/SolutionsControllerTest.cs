using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockQueryable.Moq;
using TheConsultancyFirm.Areas.Dashboard.Controllers;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using Xunit;

namespace TheConsultancyFirm.Tests.Controllers
{
    public class SolutionsControllerTest
    {
        private readonly Mock<ISolutionRepository> _solutionRepository;
        public SolutionsControllerTest()
        {
            _solutionRepository = new Mock<ISolutionRepository>();
        }

        [Fact]
        public async Task Index()
        {
            var controller = new SolutionsController(_solutionRepository.Object, null);
            var model = new List<Solution>().AsQueryable().BuildMock();
            _solutionRepository.Setup(repo => repo.GetAll()).Returns(model.Object);
            var result = controller.Index().Result;
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task SuccesfulDetailCall()
        {
            var model = new Solution
            {
                Id = 0,
                LastModified = DateTime.UtcNow,
                Title = "Title 1"
            };

            _solutionRepository.Setup(repo => repo.Get(0,true)).Returns(Task.FromResult<Solution>(model));

            var controller = new SolutionsController(_solutionRepository.Object, null);

            var result = await controller.Details(model.Id);
            
            var viewResult = Assert.IsType<ViewResult>(result);


            Assert.Equal(model, viewResult.Model);
        }
    
        [Fact]
        public async Task FailedDetailCallNull()
        {
            _solutionRepository.Setup(repo => repo.Get(0, true)).Returns(Task.FromResult<Solution>(null));

            var controller = new SolutionsController(_solutionRepository.Object, null);

            var result = await controller.Details(null);
            
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task FailedDetailCall()
        {
            _solutionRepository.Setup(repo => repo.Get(2, true)).Returns(Task.FromResult<Solution>(null));

            var controller = new SolutionsController(_solutionRepository.Object, null);

            var result = await controller.Details(2);
            
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task InvalidDetailModel()
        {
            var model = new Solution
            {
                Id = 0,
                LastModified = DateTime.UtcNow,
                Title = "Title 1"
            };

            _solutionRepository.Setup(repo => repo.Get(0, true)).Returns(Task.FromResult<Solution>(null));

            var controller = new SolutionsController(_solutionRepository.Object, null);

            var result = await controller.Details(model.Id);

            Assert.IsType<NotFoundResult>(result);
        }

    }
}
