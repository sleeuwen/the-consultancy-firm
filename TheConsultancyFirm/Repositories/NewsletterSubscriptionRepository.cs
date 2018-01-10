using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class NewsletterSubscriptionRepository : INewsletterSubscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public NewsletterSubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task Subscribe(NewsletterSubscription newsletterSubscription)
        {
            _context.Add(newsletterSubscription);
            return _context.SaveChangesAsync();
        }

        public async Task Unsubscribe(string email)
        {
            var newsLetterSubscription = await GetByMail(email);
            _context.Remove(newsLetterSubscription);
            await _context.SaveChangesAsync();
        }

        public async Task<NewsletterSubscription> GetByMail(string email)
        {
            return await _context.NewsletterSubscription.FirstOrDefaultAsync(n => n.Email == email);
        }
       
        public List<NewsletterSubscription> GetAll()
        {
            return  _context.NewsletterSubscription.ToList();
        }
    }
}
