using System.Collections.Generic;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Services
{
    public interface IRelatedItemsService
    {
        List<ContentItem> GetRelatedItems(int id, Enumeration.ContentItemType type);
    }
}
