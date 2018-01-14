using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Services
{
    public interface IMailService
    {
        Task SendAccountCreatedMailAsync(string email, string password);

        Task SendForgotPasswordMailAsync(string email, string callbackUrl);

        Task SendEmailConfirmationAsync(string email, string link);

        Task SendContactMailAsync(Contact contact);

        Task SendMailAsync(string email, string subject, string message);

        
    }
}
