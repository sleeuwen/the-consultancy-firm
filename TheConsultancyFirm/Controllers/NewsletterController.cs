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
    public class NewsletterController : Controller
    {
        private readonly INewsletterRepository _repository;

        public NewsletterController(INewsletterRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe(Newsletter newsletter)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(newsletter.Email))
            {
                return BadRequest();
            }

            try
            {
                await _repository.SubscribeAsync(newsletter);
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
