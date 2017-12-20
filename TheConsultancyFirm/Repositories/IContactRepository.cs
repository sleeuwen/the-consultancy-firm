using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface IContactRepository
    {
        Task AddAsync(Contact contact);
    }
}
