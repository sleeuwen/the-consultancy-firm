using System.Linq;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
	public interface IDownloadRepository
	{
		Download Get(int id);
		IQueryable<Download> GetAll();
	}
}