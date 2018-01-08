using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TheConsultancyFirm.Controllers
{
    public class SearchController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index(string q)
        {
            ViewBag.Search = q;

            //Todo Model stuff. Get all cases, news and downloads wich contains the search keyword.

            return View();
        }
    }
}
