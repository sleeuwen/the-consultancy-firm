using System.ComponentModel.DataAnnotations;

namespace TheConsultancyFirm.Models
{
    public class CustomerSolution
    {
        [Key]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Key]
        public int SolutionId { get; set; }
        public Solution Solution { get; set; }
    }
}
