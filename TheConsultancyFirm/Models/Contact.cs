using System.ComponentModel.DataAnnotations;

namespace TheConsultancyFirm.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Naam")]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Mobiel")]
        public string Mobile { get; set; }

        [Required]
        [Display(Name = "Onderwerp")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Bericht")]
        public string Message { get; set; }

        public bool Read { get; set; }
    }
}
