using Microsoft.AspNetCore.Mvc;

namespace TheConsultancyFirm.Controllers
{
    public class SolutionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}