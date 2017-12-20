using System.Collections.Generic;
using System.Linq;
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

		public Case Get(int id)
		{
			return _context.Cases.Include(c => c.Blocks).Include(c => c.CaseTags).ThenInclude(t => t.Tag).FirstOrDefault(c => c.Id == id);
		}

		public IQueryable<Case> GetAll()
		{
			return _context.Cases;
		}

        public List<Case> GetSurrounding(Case c)
        {
            var surroundings = new List<Case>();
            var previous = _context.Cases.OrderByDescending(i => i.Date).Where(i => i.Date < c.Date).Take(1).FirstOrDefault();
            if (previous == null)
            {
                previous = _context.Cases.OrderByDescending(i => i.Date).FirstOrDefault();
            }
            var next = _context.Cases.OrderByDescending(i => i.Date).Where(i => i.Date > c.Date).Take(1).LastOrDefault();
            if (next == null)
            {
                next = _context.Cases.OrderByDescending(i => i.Date).LastOrDefault();
            }
            surroundings.Add(previous);
            surroundings.Add(next);
            return surroundings;
        }
    }
}
