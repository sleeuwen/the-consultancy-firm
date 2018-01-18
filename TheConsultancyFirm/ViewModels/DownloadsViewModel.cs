using System.Collections.Generic;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.ViewModels
{
    public class DownloadsViewModel
    {
        public Download Selected { get; set; }
        public Download MostRecent { get; set; }
        public Download MostDownloaded { get; set; }
        public PaginatedList<Download> AllDownloads { get; set; }
    }
}
