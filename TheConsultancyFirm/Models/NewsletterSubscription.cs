using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TheConsultancyFirm.Models
{
    public class NewsletterSubscription
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [NotMapped]
        public string EncodedMail => Convert.ToBase64String(Encoding.UTF8.GetBytes(Email));
    }
}
