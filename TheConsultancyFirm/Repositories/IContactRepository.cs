using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;
using System.Collections.Generic;

namespace TheConsultancyFirm.Repositories
{
    public interface IContactRepository
    {
        Task<Contact> Get(int id);
        Task<IEnumerable<Contact>> GetAll();
        Task AddAsync(Contact contact);
        Task Update(Contact contact);
        int CountUnread();
    }
}
