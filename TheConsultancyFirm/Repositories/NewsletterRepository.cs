using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        
        public async Task<IEnumerable<Newsletter>> GetAll()
        {
            return await _context.NewsLetters.OrderByDescending(n => n.SentAt).ToListAsync();
        }

        public Task Create(Newsletter newsletter)
        {
            _context.NewsLetters.Add(newsletter);
            return _context.SaveChangesAsync();
        }
    }
}
