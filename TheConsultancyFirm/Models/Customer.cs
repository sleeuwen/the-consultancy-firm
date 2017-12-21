using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheConsultancyFirm.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoPath { get; set; }

        [Url]
        public string Link { get; set; }

        public List<Case> Cases { get; set; }
        public List<CustomerSolution> CustomerSolutions { get; set; }
    }
}
