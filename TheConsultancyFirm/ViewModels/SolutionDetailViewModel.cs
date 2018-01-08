using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.ViewModels
{
    public class SolutionDetailViewModel
    {
        public Solution Solution { get; set; }
        public List<ContentItem> ContentItems { get; set; }
        public List<CustomerSolution> Customers { get; set; }
    }
}
