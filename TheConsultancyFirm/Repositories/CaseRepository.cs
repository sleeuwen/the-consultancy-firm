using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Common;
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

        public async Task<Case> Get(int id, bool includeInactive = false)
        {
            var @case = await _context.Cases
                .Include(c => c.CaseTags).ThenInclude(t => t.Tag)
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (@case == null) return null;

            // Load only active blocks, or all if includeInactive is true
            await _context.Entry(@case)
                .Collection(c => c.Blocks)
                .Query()
                .Where(b => b.Active || includeInactive)
                .LoadAsync();

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
                               .Where(i => i.Date < c.Date && !i.Deleted && i.Enabled && i.Language == c.Language).Take(1).FirstOrDefaultAsync() ??
                           await _context.Cases.Include(i => i.Customer).OrderByDescending(i => i.Date)
                               .Where(i => i.Id != c.Id && !i.Deleted && i.Enabled && i.Language == c.Language).FirstOrDefaultAsync();

            var next = await _context.Cases.Include(i => i.Customer).OrderBy(i => i.Date).Where(i => i.Date > c.Date && !i.Deleted && i.Enabled && i.Language == c.Language)
                           .Take(1).FirstOrDefaultAsync() ??
                       await _context.Cases.Include(i => i.Customer).OrderBy(i => i.Date).Where(i => i.Id != c.Id && !i.Deleted && i.Enabled && i.Language == c.Language)
                           .FirstOrDefaultAsync();

            return (previous, next);
        }

        public async Task Create(Case @case)
        {
            _context.Cases.Add(@case);
            
            await _context.SaveChangesAsync();
            _context.ItemTranslations.Add(new ItemTranslation()
            {
                ContentType = Enumeration.ContentItemType.Case,
                IdNl = @case.Id
            });
            await _context.SaveChangesAsync();
        }

        public Task Update(Case @case)
        {
            _context.Cases.Update(@case);
            return _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var @case = await Get(id);
            _context.Cases.Remove(@case);
            await _context.SaveChangesAsync();
        }

        public Task<Case> GetLatest()
        {
            return _context.Cases.OrderByDescending(c => c.Date).Take(1).FirstOrDefaultAsync();
        }
    }
}
