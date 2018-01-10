using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IHostingEnvironment _environment;
        private readonly IMailService _mailService;

        public NewslettersController(INewsletterRepository newsletterRepository, IMailService mailService, INewsletterSubscriptionRepository newsletterSubscriptionRepository,
            IHostingEnvironment environment)
        {
            _newsletterRepository = newsletterRepository;
            _mailService = mailService;
            _newsletterSubscriptionRepository = newsletterSubscriptionRepository;
            _environment = environment;
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

        [HttpPost]
        public async Task<IActionResult> Send(Newsletter newsletter)
        {
            newsletter.SentAt = DateTime.UtcNow;
            await _newsletterRepository.Create(newsletter);
            SendMail(newsletter);
            return View();
        }

        public async Task SendMail(Newsletter newsletter)
        {
            foreach (var receiver in _newsletterSubscriptionRepository.GetAll())
            {
                var sbMail = new StringBuilder();
                using (var sReader = new StreamReader(_environment.WebRootPath+"/MailTemplate.html"))
                {
                    sbMail.Append(sReader.ReadToEnd());
                    sbMail.Replace("{subject}", newsletter.Subject);
                    sbMail.Replace("{week}", GetWeekOfYear(new DateTime()).ToString());
                    sbMail.Replace("{0}", newsletter.NewsletterIntroText);
                    sbMail.Replace("{1}", newsletter.NewsletterOtherNews);
                    sbMail.Replace("{year}", DateTime.Now.Year.ToString());
                    sbMail.Replace("{unsubscribe}", HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/newsletters/unsubscribe/" + receiver.EncodedMail);
                }

                await _mailService.SendMailAsync(receiver.Email, newsletter.Subject,
                    sbMail.ToString());
            }
        }

        public static int GetWeekOfYear(DateTime time)
        {
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);

            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) + 1;
        }

    }
}
