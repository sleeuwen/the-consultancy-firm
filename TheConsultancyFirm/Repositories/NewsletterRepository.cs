using System.Threading.Tasks;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class NewsletterRepository : INewsletterRepository
    {
        private readonly ApplicationDbContext _context;

        public NewsletterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task SubscribeAsync(Newsletter newsletter)
        {
            _context.Add(newsletter);
            return _context.SaveChangesAsync();
        }
    }
}
