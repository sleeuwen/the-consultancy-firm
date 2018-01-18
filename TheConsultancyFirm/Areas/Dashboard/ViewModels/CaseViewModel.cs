using System;
using System.Collections.Generic;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Areas.Dashboard.ViewModels
{
    public class CaseViewModel
    {
        public PaginatedList<Case> CasesList;
        public List<Tuple<int, string>> CasesWithoutTranslation;
    }
}
