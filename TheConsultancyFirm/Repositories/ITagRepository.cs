using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface ITagRepository
    {
        Task<Tag> Get(int id);
        IQueryable<Tag> GetAll();
        Task<IEnumerable<Tag>> Search(string term);
        Task Create(Tag tag);
        Task Delete(int id);
    }
}
