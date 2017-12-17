using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Controllers;
using Xunit;

namespace TheConsultancyFirm.Tests.Controllers
{
    public class HomeControllerTest
    {
        [Fact]
        public void Index()
        {
            var controller = new HomeController();
            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }
    }
}
