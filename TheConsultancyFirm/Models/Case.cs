using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using TheConsultancyFirm.Extensions;

namespace TheConsultancyFirm.Models
{
    public class Case
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Titel")]
        public string Title { get; set; }

        public string PhotoPath { get; set; }

        [Required]
        [Display(Name = "Klant")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime Date { get; set; }
        public DateTime LastModified { get; set; }

        public List<Block> Blocks { get; set; }

        public List<CaseTag> CaseTags { get; set; }

        public string Slug => $"{Id}-{Title.Sluggify()}";

        [NotMapped]
        public IFormFile Image { get; set; }

        [NotMapped]
        [Display(Name = "Tags")]
        public List<int> TagIds { get; set; }
    }
}
