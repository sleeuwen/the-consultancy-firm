using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Controllers;
using Xunit;

namespace TheConsultancyFirm.Tests.Controllers
{
    public class HomeControllerTest
    {
        [Fact]
        public void About()
        {
            var controller = new HomeController();
            var result = controller.About();

            var viewResult = Assert.IsType<ViewResult>(result);

            var message = viewResult.ViewData["Message"];
            Assert.Equal("Your application description page.", message);
        }
    }
}
