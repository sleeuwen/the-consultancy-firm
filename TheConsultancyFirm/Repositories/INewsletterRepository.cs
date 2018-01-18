using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface INewsletterRepository
    {
        IQueryable<Newsletter> GetAll();
        Task Create(Newsletter newsletter);
    }
}
