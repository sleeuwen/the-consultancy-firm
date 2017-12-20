using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TheConsultancyFirm.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error(string id)
        {
            ViewData["Title"] = "NotFound";
            ViewData["StatusError"] = id;
    }
}
