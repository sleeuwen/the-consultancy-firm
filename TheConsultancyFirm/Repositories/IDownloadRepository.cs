using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
	public interface IDownloadRepository
	{
		Task<Download> Get(int id);
		IQueryable<Download> GetAll();
		Task Create(Download download);
		Task Update(Download download);
		Task Delete(int id);
	}
}