using System.ComponentModel.DataAnnotations;

namespace TheConsultancyFirm.Models
{
    public class CaseTag
    {
        [Key]
        public int CaseId { get; set; }
        public Case Case { get; set; }

        [Key]
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
