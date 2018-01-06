using System.Collections.Generic;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.ViewModels
{
    public class NewsItemDetailViewModel
    {
        public NewsItem NewsItem { get; set; }
        public List<ContentItem> ContentItems { get; set; }
    }
}
