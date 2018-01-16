using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface ICaseRepository
    {
        Task<Case> Get(int id, bool includeInactive = false);
        IQueryable<Case> GetAll();
        Task<(Case Previous, Case Next)> GetAdjacent(Case c);
        Task<List<Case>> GetHomepageItems();
        Task Create(Case @case);
        Task Update(Case @case);
        Task Delete(int id);
        Task<Case> GetLatest();
    }
}
