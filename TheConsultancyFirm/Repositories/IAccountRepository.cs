using System.Collections.Generic;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface IAccountRepository
    {
        ApplicationUser GetUserByEmail(string email);
        void DeleteDummyUser(string email);
        Task<IEnumerable<ApplicationUser>> GetAll();
    }
}
