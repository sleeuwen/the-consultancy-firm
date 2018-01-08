using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TheConsultancyFirm.Areas.Dashboard.ViewModels
{
    public class SlideViewModel
    {
        [Required]
        public int Order { get; set; }

        public string Text { get; set; }
        public string PhotoPath { get; set; }
        public IFormFile Image { get; set; }
    }
}
