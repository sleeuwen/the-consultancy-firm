using System.Linq;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class DownloadRepository : IDownloadRepository
    {
        private ApplicationDbContext _context;

        public DownloadRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Download Get(int id)
        {
            return _context.Downloads.Include(c => c.Blocks).Include(c => c.DownloadTags).ThenInclude(t => t.Tag).FirstOrDefault(c => c.Id == id);
        }

        public IQueryable<Download> GetAll()
        {
            return _context.Downloads;
        }
    }
}
