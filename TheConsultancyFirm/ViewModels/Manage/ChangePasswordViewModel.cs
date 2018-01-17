using System.ComponentModel.DataAnnotations;

namespace TheConsultancyFirm.ViewModels.Manage
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Huidig wachtwoord")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Het {0} moet ten minste {2} en maximaal {1} karakters lang zijn.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nieuw wachtwoord")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bevestig wachtwoord")]
        [Compare("NewPassword", ErrorMessage = "De ingevoerde wachtwoorden komen niet overeen.")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }
    }
}
