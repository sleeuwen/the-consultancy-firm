using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class NewsletterRepository : INewsletterRepository
    {
        public readonly ApplicationDbContext _context;

        public NewsletterRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Newsletter>> GetAll()
        {
            return await _context.NewsLetters.ToListAsync();
        }

        public async Task Create(Newsletter newsletter)
        {
            _context.NewsLetters.Add(newsletter);
            await _context.SaveChangesAsync();
        }
    }
}
