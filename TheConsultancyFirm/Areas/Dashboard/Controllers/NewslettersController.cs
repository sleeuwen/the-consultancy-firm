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
        private readonly ICaseRepository _caseRepository;
        private readonly INewsItemRepository _newsItemRepository;
        private readonly IDownloadRepository _downloadRepository;
        private readonly INewsletterService _newsletterService;

        public NewslettersController(INewsletterRepository newsletterRepository, INewsletterSubscriptionRepository newsletterSubscriptionRepository, ICaseRepository caseRepository, INewsItemRepository newsItemRepository, IDownloadRepository downloadRepository, INewsletterService newsletterService)
        {
            _newsletterRepository = newsletterRepository;
            _newsletterSubscriptionRepository = newsletterSubscriptionRepository;
            _caseRepository = caseRepository;
            _newsItemRepository = newsItemRepository;
            _downloadRepository = downloadRepository;
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

            var _ = _newsletterService.SendNewsletter(newsletter, HttpContext.Request.Scheme+"://"+HttpContext.Request.Host,
                await _newsletterSubscriptionRepository.GetAll().ToListAsync(), await _caseRepository.GetLatest(),
                await _newsItemRepository.GetLatest(), await _downloadRepository.GetLatest());

            return View();
        }
    }
}
