using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class HomepageController : Controller
    {
        private readonly INewsItemRepository _newsItemRepository;

        public HomepageController(INewsItemRepository newsItemRepository)
        {
            _newsItemRepository = newsItemRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _newsItemRepository.GetHomepageItems());
        }

        [HttpPost("api/dashboard/[controller]/NewsItems")]
        public async Task<ObjectResult> NewsItems(string ids)
        {
            var intIds = ids.Split(",").Select(id => int.Parse(id.Trim())).ToList();
            var currentItems = await _newsItemRepository.GetHomepageItems();

            var newsItems = new List<NewsItem>();
            for (var i = 0; i < intIds.Count; i++)
            {
                var newsItem = currentItems.FirstOrDefault(n => n.Id == intIds[i]) ?? await _newsItemRepository.Get(intIds[i]);

                newsItem.HomepageOrder = i;
                await _newsItemRepository.Update(newsItem);

                newsItems.Add(newsItem);
            }

            foreach (var newsItem in currentItems.Where(n => !intIds.Contains(n.Id)))
            {
                newsItem.HomepageOrder = null;
                await _newsItemRepository.Update(newsItem);
            }

            return new ObjectResult(newsItems);
        }
    }
}
