using System.Collections.Generic;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface IRelatedItemsRepository
    {
        List<ContentItem> GetRelatedItems(int id,  Enumeration.ContentItemType type);
    }
}
