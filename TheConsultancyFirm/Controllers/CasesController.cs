using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.ViewModels;

namespace TheConsultancyFirm.Controllers
{
    public class CasesController : Controller
    {
        private readonly IRelatedItemsRepository _relatedItemsRepository;
        private readonly ICaseRepository _caseRepository;
        private readonly IItemTranslationRepository _itemTranslationRepository;

        public CasesController(IRelatedItemsRepository relatedItemsRepository, ICaseRepository caseRepository, IItemTranslationRepository itemTranslationRepository)
        {
            _relatedItemsRepository = relatedItemsRepository;
            _caseRepository = caseRepository;
            _itemTranslationRepository = itemTranslationRepository;
        }

        public async Task<IActionResult> Index()
        {
            var language = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture
                .TwoLetterISOLanguageName;
            var list = await _caseRepository.GetAll().Include(c => c.Customer).Where(c => c.Enabled && !c.Deleted && c.Language == language).OrderByDescending(c => c.Date).ToListAsync();
            return View(list);
        }

        [HttpGet("[controller]/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            // Parse everything till the first '-' as integer into `caseId`
            int.TryParse(id.Split('-', 2)[0], out int caseId);

            var caseItem = await _caseRepository.Get(caseId);
            if (caseItem == null || caseItem.Deleted || !caseItem.Enabled) return NotFound();

            var language = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture
                .TwoLetterISOLanguageName;

            if (caseItem.Language != language)
            {
                int itemTranslationId;
                itemTranslationId = language == "nl" ?
                    (await _itemTranslationRepository.GetAllCases()).FirstOrDefault(c => c.IdEn == caseItem.Id).IdNl :
                    (await _itemTranslationRepository.GetAllCases()).FirstOrDefault(c => c.IdNl == caseItem.Id).IdEn;
                caseItem = await _caseRepository.Get(itemTranslationId);
            }

            if (caseItem == null || caseItem.Deleted || !caseItem.Enabled) return NotFound();

            // Force the right slug
            if (id != caseItem.Slug)
                return RedirectToAction("Details", new {id = caseItem.Slug});

            var (previous, next) = await GetAdjacent(caseItem);
            var relatedItems = await _relatedItemsRepository.GetRelatedItems(caseItem.Id, Enumeration.ContentItemType.Case, caseItem.Language);

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
