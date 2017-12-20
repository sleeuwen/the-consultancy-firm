using Microsoft.AspNetCore.Mvc;
using Moq;
using TheConsultancyFirm.Controllers;
using TheConsultancyFirm.Models;
using Xunit;
using TheConsultancyFirm.Repositories;
using System.Net;

namespace TheConsultancyFirm.Tests.Controllers
{
    public class NewsletterControllerTest
    {
        [Fact]
        public void ValidSubscribe()
        {
            var model = new Newsletter() { Email = "info@valid.com" };

            var newsletterRepository = new Mock<INewsletterRepository>();
            newsletterRepository.Setup(news => news.Subscribe(model)).Returns(1);

            var controller = new NewsletterController(newsletterRepository.Object);
            var result = controller.Index(model);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void InvalidSubscribe()
        {
            var controller = new NewsletterController(null);

            controller.ModelState.AddModelError("Email", "Must be an email");

            var result = controller.Index(new Newsletter());
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
