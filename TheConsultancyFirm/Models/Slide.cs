using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace TheConsultancyFirm.Models
{
    public class Slide
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Text { get; set; }
        public string PhotoPath { get; set; }

        [Required]
        public CarouselBlock Block { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }
    }
}
