using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;

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

        public IActionResult Details(int id)
        {
            var caseItem = _caseRepository.Get(id);
            if (caseItem == null) return NotFound();

            _relatedItemsService.GetRelatedItems(caseItem.Id, Enumeration.ContentItemType.Case);
            return View();
        }
    }
}
