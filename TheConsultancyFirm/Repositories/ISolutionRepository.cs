using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
	public interface ISolutionRepository
	{
		Task<Solution> Get(int id);
		IQueryable<Solution> GetAll();

	    Task Add(Solution solution);
	    Task Update(Solution solution);
	    Task Delete(Solution solution);
	}
}

