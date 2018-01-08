using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
	public interface ISolutionRepository
	{
		Task<Solution> Get(int id, bool includeInactive);
		IQueryable<Solution> GetAll();

	    Task Create(Solution solution);
	    Task Update(Solution solution);
	    Task Delete(Solution solution);
	}
}

