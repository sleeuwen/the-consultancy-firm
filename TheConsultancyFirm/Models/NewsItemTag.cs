using System.ComponentModel.DataAnnotations;

namespace TheConsultancyFirm.Models
{
    public class NewsItemTag
    {
        [Key]
        public int NewsItemId { get; set; }
        public NewsItem NewsItem { get; set; }

        [Key]
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
