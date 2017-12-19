using System.ComponentModel.DataAnnotations.Schema;
using TheConsultancyFirm.Common;

namespace TheConsultancyFirm.Models
{
	[NotMapped]
    public class ContentItem
    {
	    public int Id { get; set; }
	    public Enumeration.ContentItemType Type { get; set; }
	    public int Score { get; set; }
    }
}
