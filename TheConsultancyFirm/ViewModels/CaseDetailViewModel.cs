using System.Collections.Generic;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.ViewModels
{
	public class CaseDetailViewModel
	{
		public Case CaseItem { get; set; }
		public Case Previous { get; set; }
		public Case Next { get; set; }
		public List<ContentItem> ContentItems { get; set; }
	}
}