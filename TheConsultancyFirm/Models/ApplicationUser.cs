using System;
using Microsoft.AspNetCore.Identity;

namespace TheConsultancyFirm.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime? LastLogin { get; set; }
    }
}
