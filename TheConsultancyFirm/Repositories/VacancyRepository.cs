using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class VacancyRepository : IVacancyRepository
    {
        private ApplicationDbContext _context;

        public VacancyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Vacancy> Get(int id, bool includeInactive = false)
        {
            return _context.Vacancies.FirstOrDefaultAsync(v => v.Id == id);
        }

        public IQueryable<Vacancy> GetAll()
        {
           return _context.Vacancies;
        }

        public Task Create(Vacancy vacancy)
        {
            _context.Vacancies.Add(vacancy);
            return _context.SaveChangesAsync();
        }

        public Task Update(Vacancy vacancy)
        {
            _context.Update(vacancy);
            return _context.SaveChangesAsync();
        }
    }
}
