using System.Collections.Generic;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
	public interface ITagRepository
	{
		Task<Tag> Get(int id);
		Task<IEnumerable<Tag>> GetAll();
		Task Create(Tag tag);
		Task Delete(int id);
	}
}