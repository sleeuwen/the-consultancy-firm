using System.Collections.Generic;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Areas.Dashboard.ViewModels
{
    public class HomepageViewModel
    {
        public List<Solution> Solutions { get; set; }
        public List<NewsItem> NewsItems { get; set; }
    }
}
