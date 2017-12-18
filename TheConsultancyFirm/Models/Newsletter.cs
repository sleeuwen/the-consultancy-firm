using System.ComponentModel.DataAnnotations;

namespace TheConsultancyFirm.Models
{
    public class Newsletter
    {
        public int Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
