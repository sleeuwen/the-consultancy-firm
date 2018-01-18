using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Areas.Dashboard.ViewModels;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize]
    public class HomepageController : Controller
    {
        private readonly INewsItemRepository _newsItemRepository;
        private readonly ISolutionRepository _solutionRepository;
        private readonly ICaseRepository _caseRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly IItemTranslationRepository _itemTranslationRepository;

        public HomepageController(INewsItemRepository newsItemRepository, ISolutionRepository solutionRepository, ICaseRepository caseRepository, IBlockRepository blockRepository, IItemTranslationRepository itemTranslationRepository)
        {
            _newsItemRepository = newsItemRepository;
            _solutionRepository = solutionRepository;
            _caseRepository = caseRepository;
            _blockRepository = blockRepository;
            _itemTranslationRepository = itemTranslationRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(new HomepageViewModel {
                Cases = await _caseRepository.GetHomepageItems("nl"),
                Solutions = await _solutionRepository.GetAll().ToListAsync(),
                CarouselBlock = await _blockRepository.GetHomepageCarousel(),
                NewsItems = await _newsItemRepository.GetHomepageItems("nl")
            });
        }

        [HttpPost("api/dashboard/[controller]/Cases")]
        public async Task<IActionResult> Cases(string ids)
        {
            var intIds = (ids ?? "").Split(",").Where(s => int.TryParse(s.Trim(), out var _)).Select(s => int.Parse(s.Trim())).ToList();
            var currentItems = await _caseRepository.GetHomepageItems("nl");

            var caseItems = new List<Case>();
            for (var i = 0; i < intIds.Count; i++)
            {
                var @case = currentItems.FirstOrDefault(n => n.Id == intIds[i]) ?? await _caseRepository.Get(intIds[i]);

                @case.HomepageOrder = i;
                await _caseRepository.Update(@case);

                caseItems.Add(new Case
                {
                    Id = @case.Id,
                    Title = @case.Title,
                    PhotoPath = @case.PhotoPath,
                    Customer = @case.Customer,
                    HomepageOrder = @case.HomepageOrder
                });
            }

            foreach (var @case in currentItems.Where(n => !intIds.Contains(n.Id)))
            {
                @case.HomepageOrder = null;
                await _caseRepository.Update(@case);
            }

            foreach (var item in await _caseRepository.GetHomepageItems("en"))
            {
                item.HomepageOrder = null;
                await _caseRepository.Update(item);
            }

            var caseIds = caseItems.Select(c => c.Id);
            var translatedCases = (await _itemTranslationRepository.GetAllCases())
                .Where(i => caseIds.Contains(i.IdNl)).ToDictionary(i => i.IdNl, i => i.IdEn);

            foreach (var translation in translatedCases)
            {
                var translatedCase = await _caseRepository.Get(translation.Value);
                if (translatedCase == null) continue;
                translatedCase.HomepageOrder = caseItems.First(c => c.Id == translation.Key).HomepageOrder;
                await _caseRepository.Update(translatedCase);
            }

            return View(caseItems);
        }

        [HttpPost("api/dashboard/[controller]/Solutions")]
        public async Task Solutions(string ids)
        {
            var intIds = (ids ?? "").Split(",").Where(s => int.TryParse(s.Trim(), out var _))
                .Select(s => int.Parse(s.Trim())).ToList();
            var solutions = await _solutionRepository.GetAll().ToListAsync();
            var translations = await _itemTranslationRepository.GetAllSolutions();

            for (var i = 0; i < intIds.Count; i++)
            {
                var solution = solutions.FirstOrDefault(s => s.Id == intIds[i]);
                if (solution == null) continue;

                solution.HomepageOrder = i;
                await _solutionRepository.Update(solution);

                var translation = translations.First(t => t.IdNl == intIds[i]);
                if (translation.IdEn != 0)
                {
                    var translatedSolution = solutions.First(s => s.Id == translation.IdEn);
                    if (translatedSolution == null) continue;
                    translatedSolution.HomepageOrder = i;
                    await _solutionRepository.Update(translatedSolution);
                }
            }
        }

        [HttpPost("api/dashboard/[controller]/NewsItems")]
        public async Task<IActionResult> NewsItems(string ids)
        {
            var intIds = (ids ?? "").Split(",").Where(s => int.TryParse(s.Trim(), out var _)).Select(s => int.Parse(s.Trim())).ToList();
            var currentItems = await _newsItemRepository.GetHomepageItems("nl");

            var newsItems = new List<NewsItem>();
            for (var i = 0; i < intIds.Count; i++)
            {
                var newsItem = currentItems.FirstOrDefault(n => n.Id == intIds[i]) ?? await _newsItemRepository.Get(intIds[i]);

                newsItem.HomepageOrder = i;
                await _newsItemRepository.Update(newsItem);

                newsItems.Add(new NewsItem
                {
                    Id = newsItem.Id,
                    Title = newsItem.Title,
                    Date = newsItem.Date,
                    PhotoPath = newsItem.PhotoPath,
                    HomepageOrder = newsItem.HomepageOrder
                });
            }

            foreach (var newsItem in currentItems.Where(n => !intIds.Contains(n.Id)))
            {
                newsItem.HomepageOrder = null;
                await _newsItemRepository.Update(newsItem);
            }

            foreach (var item in await _newsItemRepository.GetHomepageItems("en"))
            {
                item.HomepageOrder = null;
                await _newsItemRepository.Update(item);
            }

            var newsItemIds = newsItems.Select(n => n.Id);
            var translatedNewsItems = (await _itemTranslationRepository.GetAllNewsitems())
                .Where(n => newsItemIds.Contains(n.IdNl));

            foreach (var translation in translatedNewsItems)
            {
                var translatedNewsItem = await _newsItemRepository.Get(translation.IdEn);
                if (translatedNewsItem == null) continue;
                translatedNewsItem.HomepageOrder = newsItems.First(n => n.Id == translation.IdNl).HomepageOrder;
                await _newsItemRepository.Update(translatedNewsItem);
            }

            return View(newsItems);
        }
    }
}
