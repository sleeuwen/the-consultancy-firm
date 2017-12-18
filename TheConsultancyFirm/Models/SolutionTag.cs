using System.ComponentModel.DataAnnotations;

namespace TheConsultancyFirm.Models
{
    public class SolutionTag
    {
        [Key]
        public int SolutionId { get; set; }
        public Solution Solution { get; set; }

        [Key]
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
