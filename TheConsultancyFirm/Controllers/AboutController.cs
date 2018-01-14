using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Repositories;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TheConsultancyFirm.Controllers
{
    public class AboutController : Controller
    {
        private readonly IVacancyRepository _vacancyRepository;

        public AboutController(IVacancyRepository vacancyRepository)
        {
            _vacancyRepository = vacancyRepository;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(bool showDisabled = false)
        {
            ViewBag.ShowDisabled = showDisabled;
            return View(await _vacancyRepository.GetAll().Where(v => !v.Deleted && (v.Enabled || showDisabled))
                .OrderByDescending(v => v.VacancySince).Take(3).ToListAsync());
        }
    }
}
