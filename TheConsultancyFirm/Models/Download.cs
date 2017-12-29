using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TheConsultancyFirm.Models
{
    public class Download
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [DisplayName("Amount of downloads")]
        public int AmountOfDownloads { get; set; }
        public DateTime Date { get; set; }

        public string LinkPath { get; set; }

        [NotMapped]
        public string Filename => LinkPath?.Split('/').Last();

        [NotMapped]
        public IFormFile File { get; set; }

        public List<Block> Blocks { get; set; }
        public List<DownloadTag> DownloadTags { get; set; }
    }
}
