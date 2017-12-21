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
    }
}
