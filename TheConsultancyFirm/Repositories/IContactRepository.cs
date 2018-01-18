using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface IContactRepository
    {
        Task<Contact> Get(int id);
        IQueryable<Contact> GetAll();
        Task AddAsync(Contact contact);
        Task Update(Contact contact);
        int CountUnread();
    }
}
