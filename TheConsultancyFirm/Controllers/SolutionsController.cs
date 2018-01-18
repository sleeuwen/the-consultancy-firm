using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.ViewModels;

namespace TheConsultancyFirm.Controllers
{
    public class SolutionsController : Controller
    {
        private readonly IRelatedItemsRepository _relatedItemsRepository;
        private readonly ISolutionRepository _solutionRepository;
        private readonly IItemTranslationRepository _itemTranslationRepository;

        public SolutionsController(IRelatedItemsRepository relatedItemsRepository, ISolutionRepository solutionRepository, IItemTranslationRepository itemTranslationRepository)
        {
            _relatedItemsRepository = relatedItemsRepository;
            _solutionRepository = solutionRepository;
            _itemTranslationRepository = itemTranslationRepository;
        }

        public async Task<IActionResult> Index()
        {
            var language = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture
                .TwoLetterISOLanguageName;
            return View(await _solutionRepository.GetAll().Where(s => s.Enabled && !s.Deleted && s.Language == language).ToListAsync());
        }

        [HttpGet("[controller]/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            // Parse everything till the first '-' as integer into `solutionId`
            int.TryParse(id.Split('-', 2)[0], out int solutionId);

            var solutionItem = await _solutionRepository.Get(solutionId, false);
            if (solutionItem == null || solutionItem.Deleted || !solutionItem.Enabled) return NotFound();

            var language = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture
                .TwoLetterISOLanguageName;

            if (solutionItem.Language != language)
            {
                int itemTranslationId;
                itemTranslationId = language == "nl" ?
                    (await _itemTranslationRepository.GetAllSolutions()).FirstOrDefault(s => s.IdEn == solutionItem.Id).IdNl :
                    (await _itemTranslationRepository.GetAllSolutions()).FirstOrDefault(s => s.IdNl == solutionItem.Id).IdEn;
                solutionItem = await _solutionRepository.Get(itemTranslationId);
            }
            if (solutionItem == null || solutionItem.Deleted || !solutionItem.Enabled) return NotFound();

            if (id != solutionItem.Slug)
                return RedirectToAction("Details", new { id = solutionItem.Slug });

            var relatedItems =
                await _relatedItemsRepository.GetRelatedItems(solutionItem.Id, Enumeration.ContentItemType.Solution, language);

            var relatedCustomers = solutionItem.CustomerSolutions.Select(cs => cs.Customer).Where(c => !c.Deleted && c.Enabled).Take(12).ToList();

            return View(new SolutionDetailViewModel
            {
                Solution = solutionItem,
                ContentItems = relatedItems,
                Customers = relatedCustomers
            });
        }
    }
}
