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

		public Task<Case> Get(int id)
		{
			return _context.Cases.Include(c => c.Blocks).Include(c => c.CaseTags).ThenInclude(t => t.Tag).FirstOrDefaultAsync(c => c.Id == id);
		}

		public IQueryable<Case> GetAll()
		{
			return _context.Cases;
		}

        public async Task<(Case Previous, Case Next)> GetAdjacent (Case c)
        {
            var previous = await _context.Cases.Include(i => i.Customer).OrderByDescending(i => i.Date).Where(i => i.Date < c.Date).Take(1).FirstOrDefaultAsync() ??
                           await _context.Cases.Include(i => i.Customer).OrderByDescending(i => i.Date).Where(i => i.Id != c.Id).FirstOrDefaultAsync();

	        var next = await _context.Cases.Include(i => i.Customer).OrderBy(i => i.Date).Where(i => i.Date > c.Date).Take(1).FirstOrDefaultAsync() ??
					   await _context.Cases.Include(i => i.Customer).OrderBy(i => i.Date).Where(i => i.Id != c.Id).FirstOrDefaultAsync();

	        return (previous, next);
        }
    }
}