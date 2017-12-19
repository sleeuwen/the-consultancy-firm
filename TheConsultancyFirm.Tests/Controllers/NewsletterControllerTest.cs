using Microsoft.AspNetCore.Mvc;
using Moq;
using TheConsultancyFirm.Controllers;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;
using Xunit;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Tests.Controllers
{
    public class NewsletterControllerTest
    {
        [Fact]
        public void Index()
        {
            var controller = new NewsletterController(null);
            Newsletter newsletter = new Newsletter() { Email = "" };
            var result = controller.Index(newsletter);

            var actionResult = Assert.IsType<ActionResult>(result);
            Assert.IsType<ActionResult>(actionResult);
        }

        [Fact]
        public void ValidSubscribe()
        {
            var model = new Newsletter() { Email = "info@valid.com" };

            var newsletterRepository = new Mock<INewsletterRepository>();
            newsletterRepository.Setup(news => news.Subscribe(model)).Returns(1);

            var controller = new NewsletterController(newsletterRepository.Object);

            var result = controller.Index(model);

            Assert.Equal(IActionResult, result);
        }
    }
}
