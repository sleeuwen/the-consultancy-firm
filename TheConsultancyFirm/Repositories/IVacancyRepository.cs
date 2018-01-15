using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface IVacancyRepository
    {
        Task<Vacancy> Get(int id, bool includeInactive = false);
        Task Update(Vacancy vacancy);
        IQueryable<Vacancy> GetAll();
        Task Create(Vacancy vacancy);
    }
}
