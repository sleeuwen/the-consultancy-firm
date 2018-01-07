using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

		public async Task<Solution> Get(int id, bool includeInactive = false)
        {
            var solution = await _context.Solutions
                .Include(c => c.SolutionTags).ThenInclude(t => t.Tag)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (solution == null) return null;

            await _context.Entry(solution)
                .Collection(s => s.Blocks)
                .Query()
                .Where(b => b.Active || includeInactive)
                .LoadAsync();

            var ids = solution.Blocks.OfType<CarouselBlock>().Select(s => s.Id).ToList();
            _context.Blocks.OfType<CarouselBlock>()
                .Where(s => ids.Contains(s.Id))
                .Include(s => s.Slides)
                .Load();

            return solution;
        }

	    public IQueryable<Solution> GetAll()
	    {
	        return  _context.Solutions;
	    }

	    public Task Add(Solution solution)
	    {
	        _context.AddAsync(solution);
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

