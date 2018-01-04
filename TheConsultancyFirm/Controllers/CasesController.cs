using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;
using TheConsultancyFirm.Models;
using System.Threading.Tasks;
using TheConsultancyFirm.ViewModels;

namespace TheConsultancyFirm.Controllers
{
    public class CasesController : Controller
    {
        private readonly IRelatedItemsService _relatedItemsService;
        private readonly ICaseRepository _caseRepository;

        public CasesController(IRelatedItemsService relatedItemsService, ICaseRepository caseRepository)
        {
            _relatedItemsService = relatedItemsService;
            _caseRepository = caseRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("[controller]/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            // Parse everything till the first '-' as integer into `caseId`
            int.TryParse(id.Split('-', 2)[0], out int caseId);

            var caseItem = await _caseRepository.Get(caseId);
            if (caseItem == null) return NotFound();

            // Force the right slug
            if (id != caseItem.Slug)
                return RedirectToAction("Details", new {id = caseItem.Slug});

            var (previous, next) = await GetAdjacent(caseItem);
            var relatedItems = await _relatedItemsService.GetRelatedItems(caseItem.Id, Enumeration.ContentItemType.Case);

            return View(new CaseDetailViewModel
            {
                CaseItem = caseItem,
                ContentItems = relatedItems,
                Next = next,
                Previous = previous
            });
        }

        public async Task<(Case Previous, Case Next)> GetAdjacent(Case c)
        {
            return await _caseRepository.GetAdjacent(c);
        }
    }
}
