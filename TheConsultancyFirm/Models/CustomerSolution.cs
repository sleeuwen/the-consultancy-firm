namespace TheConsultancyFirm.Models
{
    public class CustomerSolution
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int SolutionId { get; set; }
        public Solution Solution { get; set; }
    }
}
