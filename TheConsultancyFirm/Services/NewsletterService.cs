using System;
using System.Collections.Generic;
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
        private readonly IHostingEnvironment _environment;
        private readonly IMailService _mailService;

        public NewsletterService(IMailService mailService, IHostingEnvironment environment)
        {
            _mailService = mailService;
            _environment = environment;
        }

        public async Task SendNewsletter(Newsletter newsletter, string baseUrl, List<NewsletterSubscription> subscribers, Case @case, NewsItem newsItem, Download download)
        {
            foreach (var receiver in subscribers)
            {
                var sbMail = new StringBuilder();

                using (var sReader = new StreamReader(_environment.WebRootPath + "/MailTemplate.html"))
                {
                    sbMail.Append(sReader.ReadToEnd());
                    sbMail.Replace("{subject}", newsletter.Subject);
                    sbMail.Replace("{week}", GetWeekOfYear(new DateTime()).ToString());
                    sbMail.Replace("{0}", newsletter.NewsletterIntroText);
                    sbMail.Replace("{otherNews}", newsletter.NewsletterOtherNews == null ? "" : "Ander Nieuws");
                    sbMail.Replace("{1}", newsletter.NewsletterOtherNews);
                    sbMail.Replace("{caseImage}", baseUrl + @case.PhotoPath);
                    sbMail.Replace("{caseSummary}", @case.Title);
                    sbMail.Replace("{caseLink}", baseUrl + "/cases/" + @case.Id);
                    sbMail.Replace("{newsImage}", baseUrl + newsItem.PhotoPath);
                    sbMail.Replace("{newsSummary}", newsItem.Title);
                    sbMail.Replace("{newsLink}", baseUrl + "/NewsItems/" + newsItem.Id);
                    sbMail.Replace("{downloadSummary}", download.Title);
                    sbMail.Replace("{downloadLink}", baseUrl + "/Downloads/" + download.Id);
                    sbMail.Replace("{year}", DateTime.Now.Year.ToString());
                    sbMail.Replace("{unsubscribe}", baseUrl + "/newsletters/unsubscribe/" + receiver.EncodedMail);
                }

                await _mailService.SendMailAsync(receiver.Email, newsletter.Subject, sbMail.ToString());
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
