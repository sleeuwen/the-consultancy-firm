using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.ViewModels;

namespace TheConsultancyFirm.Controllers
{
    public class NewsItemsController : Controller
    {
        private readonly IRelatedItemsRepository _relatedItemsRepository;
        private readonly INewsItemRepository _newsItemRepository;
        private readonly IItemTranslationRepository _itemTranslationRepository;

        public NewsItemsController(IRelatedItemsRepository relatedItemsRepository, INewsItemRepository newsItemRepository, IItemTranslationRepository itemTranslationRepository)
        {
            _relatedItemsRepository = relatedItemsRepository;
            _newsItemRepository = newsItemRepository;
            _itemTranslationRepository = itemTranslationRepository;
        }

        public async Task<IActionResult> Index(int? page)
        {

            var language = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture
                .TwoLetterISOLanguageName;
            var newsItems = _newsItemRepository.GetAll().Where(n => n.Enabled && !n.Deleted && n.Language == language).OrderByDescending(n => n.Date);
            return View(await PaginatedList<NewsItem>.Create(newsItems, page ?? 1, 12));
        }

        [HttpGet("[controller]/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            // Parse everything till the first '-' as integer into `caseId`
            int.TryParse(id.Split('-', 2)[0], out int newsItemId);

            var newsItem = await _newsItemRepository.Get(newsItemId);
            if (newsItem == null || newsItem.Deleted || !newsItem.Enabled) return NotFound();

            var language = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture
                .TwoLetterISOLanguageName;

            if (newsItem.Language != language)
            {
                int itemTranslationId;
                itemTranslationId = language == "nl" ?
                    (await _itemTranslationRepository.GetAllNewsitems()).FirstOrDefault(n => n.IdEn == newsItem.Id).IdNl :
                    (await _itemTranslationRepository.GetAllNewsitems()).FirstOrDefault(n => n.IdNl == newsItem.Id).IdEn;
                newsItem = await _newsItemRepository.Get(itemTranslationId);
            }
            if (newsItem == null || newsItem.Deleted || !newsItem.Enabled) return NotFound();

            // Force the right slug
            if (id != newsItem.Slug)
                return RedirectToAction("Details", new { id = newsItem.Slug });

            var relatedItems = await _relatedItemsRepository.GetRelatedItems(newsItem.Id, Enumeration.ContentItemType.NewsItem, language);

            return View(new NewsItemDetailViewModel
            {
                NewsItem = newsItem,
                ContentItems = relatedItems,
            });
        }
    }
}
