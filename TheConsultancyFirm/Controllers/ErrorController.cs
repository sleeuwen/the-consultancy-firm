using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TheConsultancyFirm.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/Index/{statusCode:int?}")]
        public IActionResult Index(int? statusCode = null)
        {
            //ViewData["Title"] = "NotFound";
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
