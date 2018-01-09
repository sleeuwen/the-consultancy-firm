using System;
using System.ComponentModel.DataAnnotations;

namespace TheConsultancyFirm.Models
{
    public class Newsletter
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Onderwerp nieuwsbrief")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Introductie")]
        public string NewsletterIntroText { get; set; }

        [Required]
        [Display(Name = "Ander nieuws")]
        public string NewsletterOtherNews { get; set; }

        [Display(Name = "Verstuurd op")]
        public DateTime SentAt { get; set; }
    }
}
