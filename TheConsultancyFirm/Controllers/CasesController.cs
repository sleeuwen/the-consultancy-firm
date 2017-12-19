using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Services;

namespace TheConsultancyFirm.Controllers
{
    public class CasesController : Controller
    {
        private IRelatedItemsService _relatedItemsService;
        private ApplicationDbContext _context;

        public CasesController(IRelatedItemsService relatedItemsService, ApplicationDbContext context)
        {
            _relatedItemsService = relatedItemsService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var caseItem = _context.Cases.Include("CaseTags").FirstOrDefault(c => c.Id == id);
            _relatedItemsService.GetRelatedItemsCase(caseItem);
            return View();
        }
    }
}
