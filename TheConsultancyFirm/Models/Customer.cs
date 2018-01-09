using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace TheConsultancyFirm.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string LogoPath { get; set; }

        [Url]
        [Required]
        public string Link { get; set; }

        public List<Case> Cases { get; set; }
        public List<CustomerSolution> CustomerSolutions { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }

        public bool Enabled { get; set; }
    }
}
