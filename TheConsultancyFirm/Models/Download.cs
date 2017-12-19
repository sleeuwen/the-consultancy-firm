using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Models
{
    public class Download
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int AmountOfDownloads { get; set; }
        public DateTime Date { get; set; }
        public string LinkPath { get; set; }

        public List<Block> Blocks { get; set; }
        public List<DownloadTag> DownloadTags { get; set; }
    }
}
