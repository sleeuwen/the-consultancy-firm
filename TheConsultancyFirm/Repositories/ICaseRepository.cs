using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface ICaseRepository
    {
        Task<Case> Get(int id);
        IQueryable<Case> GetAll();
        Task<(Case Previous, Case Next)> GetAdjacent(Case c);
        Task Create(Case @case);
        Task Update(Case @case);
        Task Delete(int id);
    }
}
