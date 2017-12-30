using System.Linq;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface ISolutionRepository
    {
        Solution Get(int id);
        IQueryable<Solution> GetAll();
    }
}
