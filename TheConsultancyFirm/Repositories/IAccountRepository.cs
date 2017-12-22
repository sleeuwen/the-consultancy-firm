using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface IAccountRepository
    {
        ApplicationUser GetUserByEmail(string email);
        void DeleteDummyUser(string email);
    }
}