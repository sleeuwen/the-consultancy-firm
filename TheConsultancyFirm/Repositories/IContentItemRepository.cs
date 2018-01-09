using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface IContentItemRepository
    {
         Task<List<ContentItem>> GetLatestItem();
    }
}
