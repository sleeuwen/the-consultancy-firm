using System.Linq;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private ApplicationDbContext _context;

        public NewsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public NewsItem Get(int id)
        {
            return _context.NewsItems.Include(c => c.Blocks).Include(c => c.NewsItemTags).ThenInclude(t => t.Tag)
                .FirstOrDefault(c => c.Id == id);
        }

        public IQueryable<NewsItem> GetAll()
        {
            return _context.NewsItems;
        }
    }
}
