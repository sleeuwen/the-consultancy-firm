using Microsoft.AspNetCore.Mvc;

namespace TheConsultancyFirm.Controllers
{
	public class CaseOverviewController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}