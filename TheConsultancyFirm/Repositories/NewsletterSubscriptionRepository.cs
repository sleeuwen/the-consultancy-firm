using System.Threading.Tasks;
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

        public Task SubscribeAsync(NewsletterSubscription newsletterSubscription)
        {
            _context.Add(newsletterSubscription);
            return _context.SaveChangesAsync();
        }
    }
}
