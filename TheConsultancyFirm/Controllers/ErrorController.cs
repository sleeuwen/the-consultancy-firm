using Microsoft.AspNetCore.Mvc;

namespace TheConsultancyFirm.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/Index/{statusCode:int?}")]
        public IActionResult Index(int? statusCode = null)
        {
            var statusText = statusCode.HasValue ? statusCode.ToString() : "error";
            ViewData["Title"] = $"Ojee, een {statusText} pagina :(";
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
