using System.Linq;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class SolutionRepository : ISolutionRepository
    {
        private ApplicationDbContext _context;

        public SolutionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Solution Get(int id)
        {
            return _context.Solutions.Include(c => c.Blocks).Include(c => c.SolutionTags).ThenInclude(t => t.Tag).FirstOrDefault(c => c.Id == id);
        }

        public IQueryable<Solution> GetAll()
        {
            return _context.Solutions;
        }
    }
}
