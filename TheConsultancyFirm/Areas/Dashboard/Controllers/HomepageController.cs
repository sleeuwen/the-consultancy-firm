using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Areas.Dashboard.ViewModels;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class HomepageController : Controller
    {
        private readonly INewsItemRepository _newsItemRepository;
        private readonly ISolutionRepository _solutionRepository;
        private readonly ICaseRepository _caseRepository;

        public HomepageController(INewsItemRepository newsItemRepository, ISolutionRepository solutionRepository, ICaseRepository caseRepository)
        {
            _newsItemRepository = newsItemRepository;
            _solutionRepository = solutionRepository;
            _caseRepository = caseRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(new HomepageViewModel {
                Cases = await _caseRepository.GetHomepageItems(),
                Solutions = await _solutionRepository.GetAll().ToListAsync(),
                NewsItems = await _newsItemRepository.GetHomepageItems()
            });
        }

        [HttpPost("api/dashboard/[controller]/NewsItems")]
        public async Task<IActionResult> NewsItems(string ids)
        {
            var intIds = (ids ?? "").Split(",").Where(s => int.TryParse(s.Trim(), out var _)).Select(s => int.Parse(s.Trim())).ToList();
            var currentItems = await _newsItemRepository.GetHomepageItems();

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
                    PhotoPath = newsItem.PhotoPath
                });
            }

            foreach (var newsItem in currentItems.Where(n => !intIds.Contains(n.Id)))
            {
                newsItem.HomepageOrder = null;
                await _newsItemRepository.Update(newsItem);
            }

            return View(newsItems);
        }

        [HttpPost("api/dashboard/[controller]/Solutions")]
        public async Task Solutions(string ids)
        {
            var intIds = (ids ?? "").Split(",").Where(s => int.TryParse(s.Trim(), out var _))
                .Select(s => int.Parse(s.Trim())).ToList();
            var solutions = await _solutionRepository.GetAll().ToListAsync();

            for (var i = 0; i < intIds.Count; i++)
            {
                var solution = solutions.FirstOrDefault(s => s.Id == intIds[i]);
                if (solution == null) continue;

                solution.HomepageOrder = i;
                await _solutionRepository.Update(solution);
            }
        }

        [HttpPost("api/dashboard/[controller]/Cases")]
        public async Task<IActionResult> Cases(string ids)
        {
            var intIds = (ids ?? "").Split(",").Where(s => int.TryParse(s.Trim(), out var _)).Select(s => int.Parse(s.Trim())).ToList();
            var currentItems = await _caseRepository.GetHomepageItems();

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
                });
            }

            foreach (var @case in currentItems.Where(n => !intIds.Contains(n.Id)))
            {
                @case.HomepageOrder = null;
                await _caseRepository.Update(@case);
            }

            return View(caseItems);
        }
    }
}
