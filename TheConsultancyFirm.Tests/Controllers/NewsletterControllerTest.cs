using Microsoft.AspNetCore.Mvc;
using Moq;
using TheConsultancyFirm.Controllers;
using TheConsultancyFirm.Models;
using Xunit;
using TheConsultancyFirm.Repositories;
using System.Threading.Tasks;

namespace TheConsultancyFirm.Tests.Controllers
{
    public class NewsletterControllerTest
    {
        [Fact]
        public async void ValidSubscribe()
        {
            var model = new Newsletter { Email = "info@valid.com" };

            var newsletterRepository = new Mock<INewsletterRepository>();
            newsletterRepository.Setup(repo => repo.SubscribeAsync(model))
                .Returns(Task.FromResult(1));

            var controller = new NewsletterController(newsletterRepository.Object);
            var result = await controller.Subscribe(model);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void InvalidSubscribe()
        {
            var controller = new NewsletterController(null);

            controller.ModelState.AddModelError("Email", "Must be an email");

            var result = await controller.Subscribe(new Newsletter());
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
