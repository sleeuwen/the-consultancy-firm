namespace TheConsultancyFirm.Models
{
    public class SolutionTag
    {
        public int SolutionId { get; set; }
        public Solution Solution { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
