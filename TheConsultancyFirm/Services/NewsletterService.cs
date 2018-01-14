using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Services
{
    public class NewsletterService : INewsletterService
    {
        private readonly INewsletterSubscriptionRepository _newsletterSubscriptionRepository;
        private readonly IHostingEnvironment _environment;
        private readonly IMailService _mailService;
        private readonly ICaseRepository _caseRepository;
        private readonly INewsItemRepository _newsItemRepository;
        private readonly IDownloadRepository _downloadRepository;

        public NewsletterService(IMailService mailService, INewsletterSubscriptionRepository newsletterSubscriptionRepository,
            IHostingEnvironment environment, ICaseRepository caseRepository, INewsItemRepository newsItemRepository, IDownloadRepository downloadRepository)
        {
            _mailService = mailService;
            _newsletterSubscriptionRepository = newsletterSubscriptionRepository;
            _environment = environment;
            _caseRepository = caseRepository;
            _newsItemRepository = newsItemRepository;
            _downloadRepository = downloadRepository;
        }

        public async Task SendNewsletter(Newsletter newsletter, string baseUrl)
        {
            var @case = await _caseRepository.GetLatest();
            var newsItem = await _newsItemRepository.GetLatest();
            var download = await _downloadRepository.GetLatest();

            foreach (var receiver in _newsletterSubscriptionRepository.GetAll())
            {
                var sbMail = new StringBuilder();

                using (var sReader = new StreamReader(_environment.WebRootPath + "/MailTemplate.html"))
                {
                    sbMail.Append(sReader.ReadToEnd());
                    sbMail.Replace("{subject}", newsletter.Subject);
                    sbMail.Replace("{week}", GetWeekOfYear(new DateTime()).ToString());
                    sbMail.Replace("{0}", newsletter.NewsletterIntroText);
                    if (newsletter.NewsletterOtherNews == null)
                    {
                        sbMail.Replace("{otherNews}", "");
                    }
                    sbMail.Replace("{1}", newsletter.NewsletterOtherNews);
                    sbMail.Replace("{caseImage}", @case.PhotoPath);
                    sbMail.Replace("{caseSummary}", @case.Title);
                    sbMail.Replace("{caseLink}", baseUrl + "/cases/" + @case.Id);
                    sbMail.Replace("{newsImage}", newsItem.PhotoPath);
                    sbMail.Replace("{newsSummary}", newsItem.Title);
                    sbMail.Replace("{newsLink}", baseUrl + "/NewsItems/" + newsItem.Id);
                    sbMail.Replace("{downloadSummary}", download.Title);
                    sbMail.Replace("{downloadLink}", baseUrl + "/Downloads/" + download.Id);
                    sbMail.Replace("{year}", DateTime.Now.Year.ToString());
                    sbMail.Replace("{unsubscribe}", baseUrl + "/newsletters/unsubscribe/" + receiver.EncodedMail);
                }

                await _mailService.SendMailAsync(receiver.Email, newsletter.Subject,
                    sbMail.ToString());
            }
        }

        private static int GetWeekOfYear(DateTime time)
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
