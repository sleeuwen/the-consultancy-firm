using System.Collections.Generic;
using System.Threading.Tasks;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface IRelatedItemsRepository
    {
        Task<List<ContentItem>> GetRelatedItems(int id, Enumeration.ContentItemType type);
    }
}
