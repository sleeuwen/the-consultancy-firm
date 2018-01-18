using System;
using System.Collections.Generic;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Areas.Dashboard.ViewModels
{
    public class DownloadViewModel
    {
        public PaginatedList<Download> DownloadsList;
        public List<Tuple<int, string>> DownloadsWithoutTranslation;
    }
}
