using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class SolutionsController : Controller
    {
        private readonly ISolutionRepository _solutionRepository;

        public SolutionsController(ISolutionRepository solutionRepository)
        {
            _solutionRepository = solutionRepository;
        }

        // GET: Dashboard/Solutions
        public async Task<IActionResult> Index()
        {
            return View(_solutionRepository.GetAll());
        }

        // GET: Dashboard/Solutions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solution = await _solutionRepository.Get((int)id);
            if (solution == null)
            {
                return NotFound();
            }

            return View(solution);
        }

        // GET: Dashboard/Solutions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Solutions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Date,LastModified")] Solution solution)
        {
            solution.LastModified = DateTime.UtcNow;
            solution.Date = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                await _solutionRepository.Add(solution);
                return RedirectToAction(nameof(Index));
            }
            return View(solution);
        }

        // GET: Dashboard/Solutions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solution = await _solutionRepository.Get((int) id);
            if (solution == null)
            {
                return NotFound();
            }
            return View(solution);
        }

        // POST: Dashboard/Solutions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Date,LastModified")] Solution solution)
        {
            if (id != solution.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                solution.LastModified = DateTime.UtcNow;
                try
                {
                    await _solutionRepository.Update(solution);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SolutionExists(solution.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(solution);
        }

        // GET: Dashboard/Solutions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solution = await _solutionRepository.Get((int)id);
            if (solution == null)
            {
                return NotFound();
            }

            return View(solution);
        }

        // POST: Dashboard/Solutions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var solution = await _solutionRepository.Get((int)id);
            await _solutionRepository.Delete(solution);
            return RedirectToAction(nameof(Index));
        }

        private bool SolutionExists(int id)
        {
            return _solutionRepository.Get(id) == null;
        }
    }
}
