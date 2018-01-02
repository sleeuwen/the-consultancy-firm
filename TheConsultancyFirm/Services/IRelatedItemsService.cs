using System.Collections.Generic;
using System.Threading.Tasks;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Services
{
    public interface IRelatedItemsService
    {
        Task<List<ContentItem>> GetRelatedItems(int id, Enumeration.ContentItemType type);
    }
}
