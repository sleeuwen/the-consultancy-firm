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

        public HomeController(ICustomerRepository customerRepository, INewsItemRepository newsItemRepository, ISolutionRepository solutionRepository)
        {
            _customerRepository = customerRepository;
            _newsItemRepository = newsItemRepository;
            _solutionRepository = solutionRepository;
        }

        public async Task<IActionResult> Index()
        {
            var customers = (await _customerRepository.GetAll()).Where(c => c.Enabled && !c.Deleted).Take(12).ToList();
            var solutions = await _solutionRepository.GetAll().OrderBy(s => s.HomepageOrder).ToListAsync();
            var newsItems = await _newsItemRepository.GetHomepageItems();

            return View(new HomeViewModel
            {
                Customers = customers,
                Solutions = solutions,
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
