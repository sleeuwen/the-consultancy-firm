using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class VacanciesController : Controller
    {
        private readonly IVacancyRepository _vacancyRepository;

        public VacanciesController(IVacancyRepository vacancyRepository)
        {
            _vacancyRepository = vacancyRepository;
        }

        // GET: Dashboard/Vacancies
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page, bool showDisabled = false)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewBag.ShowDisabled = showDisabled;
            var cases = _vacancyRepository.GetAll().Where(c => !c.Deleted && (c.Enabled || showDisabled) && (string.IsNullOrEmpty(searchString) || c.FunctionDescription.Contains(searchString)));

            switch (sortOrder)
            {
                case "name_desc":
                    cases = cases.OrderByDescending(c => c.FunctionDescription);
                    break;
                case "Date":
                    cases = cases.OrderBy(c => c.VacancySince);
                    break;
                case "date_desc":
                    cases = cases.OrderByDescending(c => c.VacancySince);
                    break;
                default:
                    cases = cases.OrderBy(c => c.FunctionDescription);
                    break;
            }
            return View(await PaginatedList<Vacancy>.Create(cases.AsQueryable(), page ?? 1, 5));
        }

        // GET: Dashboard/Vacancies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Vacancies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FunctionDescription,VacancySince")] Vacancy vacancy)
        {
            if (ModelState.IsValid)
            {
                vacancy.Enabled = true;
                await _vacancyRepository.Create(vacancy);
                return RedirectToAction(nameof(Index));
            }
            return View(vacancy);
        }

        public async Task<IActionResult> ToggleEnable(int? id)
        {
            var vacancy = await _vacancyRepository.Get(id ?? 0);
            if (vacancy == null)
            {
                return NotFound();
            }

            vacancy.Enabled = !vacancy.Enabled;

            await _vacancyRepository.Update(vacancy);
            return RedirectToAction(nameof(Index));
        }

        // GET: Dashboard/Solutions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacancy = await _vacancyRepository.Get((int)id, true);
            if (vacancy == null)
            {
                return NotFound();
            }

            return View(vacancy);
        }

        // POST: Dashboard/Solutions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var vacancy = await _vacancyRepository.Get(id ?? 0, true);
            if (vacancy == null)
            {
                return NotFound();
            }

            vacancy.Deleted = true;

            await _vacancyRepository.Update(vacancy);
            return RedirectToAction(nameof(Index));
        }
    }
}
