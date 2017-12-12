using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Haalt met GET de homepagina op
        /// </summary>
        /// <returns>De homepagina -> Index.cshtml</returns>
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}