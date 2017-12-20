using System.Collections.Generic;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.ViewModels
{
	public class CaseDetailViewModel
	{
		public Case CaseItem { get; set; }
		public List<Case> Surroundings { get; set; }
		public List<ContentItem> ContentItems { get; set; }
	}
}