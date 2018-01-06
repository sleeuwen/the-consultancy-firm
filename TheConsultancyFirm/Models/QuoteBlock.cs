using System.ComponentModel.DataAnnotations;

namespace TheConsultancyFirm.Models
{
    public class QuoteBlock : Block
    {
        [Required]
        public string Text { get; set; }
        public string Author { get; set; }
    }
}
