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
        
        public IQueryable<Newsletter> GetAll()
        {
            return _context.NewsLetters;
        }

        public Task Create(Newsletter newsletter)
        {
            _context.NewsLetters.Add(newsletter);
            return _context.SaveChangesAsync();
        }
    }
}
