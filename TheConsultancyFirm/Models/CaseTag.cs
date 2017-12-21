namespace TheConsultancyFirm.Models
{
    public class CaseTag
    {
        public int CaseId { get; set; }
        public Case Case { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
