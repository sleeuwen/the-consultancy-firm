using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Services
{
    public interface IMailService
    {
        Task SendContactMailAsync(Contact contact);

        Task SendMailAsync(string email, string subject, string message);
    }
}
