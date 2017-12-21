using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Controllers;
using Xunit;

namespace TheConsultancyFirm.Tests.Controllers
{
    public class ErrorControllerTest
    {
        [Fact]
        public void Index()
        {
            var controller = new ErrorController();
            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }
    }
}
