using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Models
{
    public class Case
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Customer Customer { get; set; }
        public DateTime Date { get; set; }
        public DateTime LastModified { get; set; }

        public List<Block> Blocks { get; set; }
        public List<CaseTag> CaseTags { get; set; }
    }
}
