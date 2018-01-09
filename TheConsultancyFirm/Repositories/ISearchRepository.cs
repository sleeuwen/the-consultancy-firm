using System.Collections.Generic;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface ISearchRepository
    {
        Task<IEnumerable<ContentItem>> Search(string term);
    }
}
