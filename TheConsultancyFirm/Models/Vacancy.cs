using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheConsultancyFirm.Models
{
    public class Vacancy
    {
        public int Id { get; set; }

        [Display(Name="Functieomschrijving")]
        public string FunctionDescription { get; set; }

        [Display(Name = "Vacature sinds")]
        public DateTime VacancySince { get; set; }

        public bool Enabled { get; set; }
        public bool Deleted { get; set; }
    }
}
