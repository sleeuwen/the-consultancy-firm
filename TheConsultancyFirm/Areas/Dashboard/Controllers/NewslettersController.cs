using System;
using System.Linq;
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

        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["SubjectSortParm"] = sortOrder == "Subject" ? "sub_desc" : "Subject";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            var contacts =  _newsletterRepository.GetAll().Where(c => string.IsNullOrEmpty(searchString) || c.Subject.Contains(searchString) || c.NewsletterIntroText.Contains(searchString));

            switch (sortOrder)
            {
                case "Date":
                    contacts = contacts.OrderBy(c => c.SentAt);
                    break;
                case "Subject":
                    contacts = contacts.OrderBy(c => c.Subject);
                    break;
                case "sub_desc":
                    contacts = contacts.OrderByDescending(c => c.Subject);
                    break;
                default:
                    contacts = contacts.OrderByDescending(c => c.SentAt);
                    break;
            }
            return View(await PaginatedList<Newsletter>.Create(contacts, page ?? 1, 5));
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
