using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.ViewModels;

namespace TheConsultancyFirm.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly INewsItemRepository _newsItemRepository;
        private readonly ISolutionRepository _solutionRepository;
        private readonly ICaseRepository _caseRepository;
        private readonly IBlockRepository _blockRepository;

        public HomeController(ICustomerRepository customerRepository, INewsItemRepository newsItemRepository, ISolutionRepository solutionRepository, ICaseRepository caseRepository, IBlockRepository blockRepository)
        {
            _customerRepository = customerRepository;
            _newsItemRepository = newsItemRepository;
            _solutionRepository = solutionRepository;
            _caseRepository = caseRepository;
            _blockRepository = blockRepository;
        }

        public async Task<IActionResult> Index()
        {
            var language = HttpContext?.Request?.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US" ? "en" : "nl";

            var customers = (await _customerRepository.GetAll()).Where(c => c.Enabled && !c.Deleted).Take(12).ToList();
            var cases = await _caseRepository.GetHomepageItems();
            var solutions = await _solutionRepository.GetAll().Where(s => s.Language == language).OrderBy(s => s.HomepageOrder).ToListAsync();
            var carousel = await _blockRepository.GetHomepageCarousel();
            var newsItems = await _newsItemRepository.GetHomepageItems();

            return View(new HomeViewModel
            {
                Customers = customers,
                Cases = cases,
                Solutions = solutions,
                CarouselBlock = carousel,
                NewsItems = newsItems,
            });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
