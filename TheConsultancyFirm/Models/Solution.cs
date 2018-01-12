using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using TheConsultancyFirm.Extensions;

namespace TheConsultancyFirm.Models
{
    public class Solution
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public DateTime Date { get; set; }
        public DateTime LastModified { get; set; }

        public string PhotoPath { get; set; }
       
        public List<Block> Blocks { get; set; }
        public List<SolutionTag> SolutionTags { get; set; }
        public List<CustomerSolution> CustomerSolutions { get; set; }

        public string Slug => $"{Id}-{Title.Sluggify()}";

        [NotMapped]
        public IFormFile Image { get; set; }

        [NotMapped]
        [Display(Name = "Tags")]
        public List<int> TagIds { get; set; }

        [NotMapped]
        [Display(Name="Klanten")]
        public List<int> CustomerIds { get; set; }

        [MaxLength(140)]
        [Display(Name = "Omschrijving voor delen (max 140 char)")]
        public string SharingDescription { get; set; }

        [Required]
        [MaxLength(300)]
        [Display(Name = "Samenvatting (max 300 char)")]
        public string Summary { get; set; }

        public bool Enabled { get; set; }
        public bool Deleted { get; set; }
    }
}
