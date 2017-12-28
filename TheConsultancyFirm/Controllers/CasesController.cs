using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;
using System.Linq;
using TheConsultancyFirm.Models;
using System.Collections.Generic;
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
            int.TryParse(id.Substring(0, id.IndexOf('-') < 0 ? id.Length : id.IndexOf('-')), out int caseId);

            var caseItem = await _caseRepository.Get(caseId);
            if (caseItem == null) return NotFound();

            if (id != caseItem.Slug)
                return RedirectToAction("Details", new {id = caseItem.Slug});

            var adjacents = await GetAdjacent(caseItem);

            var relatedItems = _relatedItemsService.GetRelatedItems(caseItem.Id, Enumeration.ContentItemType.Case);

	        var model = new CaseDetailViewModel
	        {
		        CaseItem = caseItem,
				ContentItems = relatedItems,
				Next = adjacents.Next,
				Previous = adjacents.Previous
			};
            return View(model);
        }

        public async Task<(Case Previous, Case Next)> GetAdjacent(Case c)
        {
            return await _caseRepository.GetAdjacent(c);
        }
    }
}
