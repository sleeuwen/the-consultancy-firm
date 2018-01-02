using System.Linq;
using System.Threading.Tasks;
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

        public Task<Solution> Get(int id)
        {
            return _context.Solutions.Include(c => c.Blocks).Include(c => c.SolutionTags).ThenInclude(t => t.Tag)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<Solution> GetAll()
        {
            return _context.Solutions;
        }

        public Task Add(Solution solution)
        {
            _context.Add(solution);
            return _context.SaveChangesAsync();
        }

        public Task Update(Solution solution)
        {
            _context.Update(solution);
            return _context.SaveChangesAsync();
        }

        public Task Delete(Solution solution)
        {
            _context.Remove(solution);
            return _context.SaveChangesAsync();
        }
    }
}
