using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;
using System.Collections.Generic;

namespace TheConsultancyFirm.Repositories
{
    public interface IContactRepository
    {
        Task AddAsync(Contact contact);
        IEnumerable<Contact> GetAll();
        Task Update(Contact contact);
        int CountUnreaded();
    }
}
