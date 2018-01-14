using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface INewsletterSubscriptionRepository
    {
        Task Subscribe(NewsletterSubscription newsletterSubscription);
        Task Unsubscribe(string email);
        Task<NewsletterSubscription> GetByMail(string email);

        IQueryable<NewsletterSubscription> GetAll();
    }
}
