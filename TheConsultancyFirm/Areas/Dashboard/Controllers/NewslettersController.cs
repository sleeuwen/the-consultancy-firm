using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class NewslettersController : Controller
    {
        private readonly INewsletterRepository _newsletterRepository;
        private readonly INewsletterSubscriptionRepository _newsletterSubscriptionRepository;
        private readonly INewsletterService _newsletterService;

        public NewslettersController(INewsletterRepository newsletterRepository, INewsletterSubscriptionRepository newsletterSubscriptionRepository, INewsletterService newsletterService)
        {
            _newsletterRepository = newsletterRepository;
            _newsletterSubscriptionRepository = newsletterSubscriptionRepository;
            _newsletterService = newsletterService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _newsletterRepository.GetAll());
        }

        // GET: Dashboard/Newsletter/Create
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Subscriptions()
        {
            return View(await _newsletterSubscriptionRepository.GetAll().ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Send(Newsletter newsletter)
        {
            newsletter.SentAt = DateTime.UtcNow;
            await _newsletterRepository.Create(newsletter);
            _newsletterService.SendNewsletter(newsletter, HttpContext.Request.Scheme+"://"+HttpContext.Request.Host);
            return View();
        }
    }
}
