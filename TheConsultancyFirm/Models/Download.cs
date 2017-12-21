using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheConsultancyFirm.Models
{
    public class Download
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int AmountOfDownloads { get; set; }
        public DateTime Date { get; set; }

        [Url]
        public string LinkPath { get; set; }

            [NotMapped]
            public IFormFile File { get; set; }

        public List<Block> Blocks { get; set; }
        public List<DownloadTag> DownloadTags { get; set; }
    }
}
