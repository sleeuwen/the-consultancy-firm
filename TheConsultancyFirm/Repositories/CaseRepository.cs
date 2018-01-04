using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class CaseRepository : ICaseRepository
    {
        private ApplicationDbContext _context;

        public CaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Case> Get(int id)
        {
            var @case = await _context.Cases.Include(c => c.Blocks).Include(c => c.CaseTags).ThenInclude(t => t.Tag)
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.Id == id);

            // Load the slides from all CarouselBlock's
            var ids = @case.Blocks.OfType<CarouselBlock>().Select(c => c.Id).ToList();
            _context.Blocks.OfType<CarouselBlock>()
                .Where(c => ids.Contains(c.Id))
                .Include(c => c.Slides)
                .Load();

            return @case;
        }

        public IQueryable<Case> GetAll()
        {
            return _context.Cases;
        }

        public async Task<(Case Previous, Case Next)> GetAdjacent(Case c)
        {
            var previous = await _context.Cases.Include(i => i.Customer).OrderByDescending(i => i.Date)
                               .Where(i => i.Date < c.Date).Take(1).FirstOrDefaultAsync() ??
                           await _context.Cases.Include(i => i.Customer).OrderByDescending(i => i.Date)
                               .Where(i => i.Id != c.Id).FirstOrDefaultAsync();

            var next = await _context.Cases.Include(i => i.Customer).OrderBy(i => i.Date).Where(i => i.Date > c.Date)
                           .Take(1).FirstOrDefaultAsync() ??
                       await _context.Cases.Include(i => i.Customer).OrderBy(i => i.Date).Where(i => i.Id != c.Id)
                           .FirstOrDefaultAsync();

            return (previous, next);
        }
    }
}
