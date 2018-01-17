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
    public class NewsItemsControllerTest
    {
        private readonly Mock<INewsItemRepository> _newsItemsRepository;
        private readonly Mock<IItemTranslationRepository> _itemTranslationRepository;

        public NewsItemsControllerTest()
        {
            _newsItemsRepository = new Mock<INewsItemRepository>();
            _itemTranslationRepository = new Mock<IItemTranslationRepository>();
        }

        [Fact]
        public async Task Index()
        {
            var controller = new NewsItemsController(_newsItemsRepository.Object, null, _itemTranslationRepository.Object);
            var model = new List<NewsItem>().AsQueryable().BuildMock();
            _newsItemsRepository.Setup(repo => repo.GetAll()).Returns(model.Object);
            var result = await controller.Index();
            Assert.IsType<ViewResult>(result);
        }
    }
}
