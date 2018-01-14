using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Services
{
    public interface INewsletterService
    {
        Task SendNewsletter(Newsletter newsletter, string baseUrl);
    }
}
