using System;
using System.Collections.Generic;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Areas.Dashboard.ViewModels
{
    public class NewsItemViewModel
    {
        public PaginatedList<NewsItem> NewsItemsList;
        public List<Tuple<int, string>> NewsItemsWithoutTranslation;
    }
}
