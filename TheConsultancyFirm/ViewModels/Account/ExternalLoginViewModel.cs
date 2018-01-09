using System.ComponentModel.DataAnnotations;

namespace TheConsultancyFirm.ViewModels.Account
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
