using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TheConsultancyFirm.Controllers;
using TheConsultancyFirm.Repositories;
using Xunit;

namespace TheConsultancyFirm.Tests.Controllers
{
    public class SearchControllerTest
    {
        private Mock<ISearchRepository> _searchRepository;

        public SearchControllerTest()
        {
            _searchRepository = new Mock<ISearchRepository>();
        }

        [Fact]
        public async Task Index()
        {
            var controller = new SearchController(_searchRepository.Object);
            var result = await controller.Index("");

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void NumberWithQuote()
        {
            var controller = new SearchController(_searchRepository.Object);
            string s = "'2";
            var result = controller.Index(s).IsCompletedSuccessfully;

            Assert.True(result);
        }
        [Fact]
        public void NumberWithQuote2()
        {
            var controller = new SearchController(_searchRepository.Object);
            string s = "2'";
            var result = controller.Index(s).IsCompletedSuccessfully;

            Assert.True(result);
        }
        [Fact]
        public void Quote()
        {
            var controller = new SearchController(_searchRepository.Object);
            string s = "'";
            var result = controller.Index(s).IsCompletedSuccessfully;

            Assert.True(result);
        }
    }
}
