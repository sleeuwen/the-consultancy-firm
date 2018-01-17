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

        public HomeController(ICustomerRepository customerRepository, INewsItemRepository newsItemRepository)
        {
            _customerRepository = customerRepository;
            _newsItemRepository = newsItemRepository;
        }

        public async Task<IActionResult> Index()
        {
            var customers = (await _customerRepository.GetAll()).Where(c => c.Enabled && !c.Deleted).Take(12).ToList();
            var newsItems = await _newsItemRepository.GetAll().Where(c => c.Enabled && !c.Deleted).Take(3).ToListAsync();


            return View(new HomeViewModel
            {
                Customers = customers,
                NewsItems = newsItems
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
