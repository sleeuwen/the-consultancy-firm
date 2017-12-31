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

        public async Task<IActionResult> Details(int id)
        {
            var caseItem = await _caseRepository.Get(id);
            if (caseItem == null) return NotFound();

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
