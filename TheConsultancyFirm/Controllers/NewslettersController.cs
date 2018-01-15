using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Controllers
{
    public class NewslettersController : Controller
    {
        private readonly INewsletterSubscriptionRepository _newsletterSubscriptionRepository;

        public NewslettersController(INewsletterSubscriptionRepository newsletterSubscriptionRepository)
        {
            _newsletterSubscriptionRepository = newsletterSubscriptionRepository;
        }

        public async Task<IActionResult> Unsubscribe(string id)
        {
            var data = Convert.FromBase64String(id);
            var email = Encoding.UTF8.GetString(data);
            await _newsletterSubscriptionRepository.Unsubscribe(email);

            return RedirectToAction("SuccesfulUnsubscription");
        }

        public IActionResult SuccesfulUnsubscription()
        {
            return View();
        }
    }
}
