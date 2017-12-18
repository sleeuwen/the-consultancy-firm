using System.ComponentModel.DataAnnotations;

namespace TheConsultancyFirm.Models
{
    public class DownloadTag
    {
        [Key]
        public int DownloadId { get; set; }
        public Download Download { get; set; }

        [Key]
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
