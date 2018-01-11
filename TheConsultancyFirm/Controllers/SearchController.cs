using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchRepository _searchRepository;

        public SearchController(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public async Task<IActionResult> Index(string q)
        {
            ViewBag.Search = q;

            return View(await _searchRepository.Search(q));
        }
    }
}
