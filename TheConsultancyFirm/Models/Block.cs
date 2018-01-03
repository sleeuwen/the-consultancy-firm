using System;

namespace TheConsultancyFirm.Models
{
    public class Block
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public bool Active { get; set; }
        public DateTime Date { get; set; }
        public DateTime LastModified { get; set; }

        public int? CaseId { get; set; }
        public int? NewsItemId { get; set; }
        public int? SolutionId { get; set; }
    }
}
