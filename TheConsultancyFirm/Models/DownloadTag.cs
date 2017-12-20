namespace TheConsultancyFirm.Models
{
    public class DownloadTag
    {
        public int DownloadId { get; set; }
        public Download Download { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
