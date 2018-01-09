using System.ComponentModel.DataAnnotations;

namespace TheConsultancyFirm.Models
{
    public class NewsletterSubscription
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
