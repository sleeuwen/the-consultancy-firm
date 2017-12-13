using Microsoft.AspNetCore.Mvc;

namespace TheConsultancyFirm.Controllers
{
    public class CasesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}