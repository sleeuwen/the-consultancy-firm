using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Controllers;
using Xunit;

namespace TheConsultancyFirm.Tests.Controllers
{
    [Area("Dashboard")]
    public class DownloadsControllerTest
    {
        [Fact]
        public void Index()
        {
            var controller = new DownloadsController(null, null);
            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }
    }
}
