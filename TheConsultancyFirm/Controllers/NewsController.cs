using Microsoft.AspNetCore.Mvc;

namespace TheConsultancyFirm.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}