using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Controllers
{
    [Route("api/[controller]")]
    public class NewsletterSubscriptionController : Controller
    {
        private readonly INewsletterSubscriptionRepository _newsletterSubscriptionRepository;

        public NewsletterSubscriptionController(INewsletterSubscriptionRepository newsletterSubscriptionRepository)
        {
            _newsletterSubscriptionRepository = newsletterSubscriptionRepository;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe(NewsletterSubscription newsletterSubscription)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(newsletterSubscription.Email))
            {
                return BadRequest();
            }

            try
            {
                await _newsletterSubscriptionRepository.Subscribe(newsletterSubscription);
                return Ok();
            }
            catch (DbUpdateException)
            {
                // Hide the fact the user is already subscribed.
                return Ok();
            }
        }
    }
}
