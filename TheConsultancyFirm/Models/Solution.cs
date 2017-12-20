using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Models
{
    public class Solution
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public DateTime LastModified { get; set; }

        public List<Block> Blocks { get; set; }
        public List<SolutionTag> SolutionTags { get; set; }
        public List<CustomerSolution> CustomerSolutions { get; set; }
    }
}
