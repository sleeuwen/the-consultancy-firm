using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;
using TheConsultancyFirm.ViewModels;

namespace TheConsultancyFirm.Controllers
{
    public class NewsController : Controller
    {
        private readonly IRelatedItemsService _relatedItemsService;
        private readonly INewsItemRepository _newsItemRepository;

        public NewsController(IRelatedItemsService relatedItemsService, INewsItemRepository newsItemRepository)
        {
            _relatedItemsService = relatedItemsService;
            _newsItemRepository = newsItemRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("[controller]/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            // Parse everything till the first '-' as integer into `caseId`
            int.TryParse(id.Split('-', 2)[0], out int newsItemId);

            var newsItem = await _newsItemRepository.Get(newsItemId);
            if (newsItem == null) return NotFound();

            // Force the right slug
            if (id != newsItem.Slug)
                return RedirectToAction("Details", new { id = newsItem.Slug });

            var relatedItems = await _relatedItemsService.GetRelatedItems(newsItem.Id, Enumeration.ContentItemType.NewsItem);

            return View(new NewsItemDetailViewModel
            {
                NewsItem = newsItem,
                ContentItems = relatedItems,
            });
        }
    }
}
