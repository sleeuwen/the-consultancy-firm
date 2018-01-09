using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class HomepageController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
