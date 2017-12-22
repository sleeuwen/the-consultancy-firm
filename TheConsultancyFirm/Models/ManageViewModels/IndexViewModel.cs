using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheConsultancyFirm.Models.ManageViewModels
{
    public class IndexViewModel
    {
        [Display(Name = "Gebruikersnaam")]
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Telefoonnummer")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }
    }
}
