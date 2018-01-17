using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Areas.Dashboard.ViewModels
{
    public class NewsletterViewModel
    {
        public Case LatestCase { get; set; }
        public NewsItem LatestNewsItem { get; set; }
        public Download LatesDownload { get; set; }
    }
}
