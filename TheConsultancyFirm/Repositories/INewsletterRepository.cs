using System.Collections.Generic;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface INewsletterRepository
    {
        Task<IEnumerable<Newsletter>> GetAll();

        Task SubscribeAsync(Newsletter newsletter);
    }
}
