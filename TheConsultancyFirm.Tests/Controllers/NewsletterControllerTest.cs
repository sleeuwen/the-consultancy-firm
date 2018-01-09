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
            var model = new NewsletterSubscription {Email = "info@valid.com"};

            var newsletterRepository = new Mock<INewsletterSubscriptionRepository>();
            newsletterRepository.Setup(repo => repo.SubscribeAsync(model))
                .Returns(Task.FromResult(1));

            var controller = new NewsletterSubscriptionController(newsletterRepository.Object);
            var result = await controller.Subscribe(model);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void InvalidSubscribe()
        {
            var controller = new NewsletterSubscriptionController(null);

            controller.ModelState.AddModelError("Email", "Must be an email");

            var result = await controller.Subscribe(new NewsletterSubscription());
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
