using System.ComponentModel.DataAnnotations.Schema;
using TheConsultancyFirm.Common;

namespace TheConsultancyFirm.Models
{
    [NotMapped]
    public class ContentItem
    {
        public int Id { get; set; }
        public Enumeration.ContentItemType Type { get; set; }
        public double Score { get; set; }
        public string PhotoPath { get; set; }
        public string Titel { get; set; }
        public string TypeString => Type == Enumeration.ContentItemType.NewsItem ? "Nieuws" : Type.ToString();
        public string LinkPath { get; set; }
    }
}
