using System.Collections.Generic;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Services
{
    public interface INewsletterService
    {
        Task SendNewsletter(Newsletter newsletter, string baseUrl, List<NewsletterSubscription> subscribers, Case @case, NewsItem newsItem, Download download);
    }
}
