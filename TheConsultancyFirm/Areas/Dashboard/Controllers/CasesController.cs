using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class CasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICaseRepository _caseRepository;

        public CasesController(ApplicationDbContext context, ICaseRepository caseRepository)
        {
            _context = context;
            _caseRepository = caseRepository;
        }

        // GET: Dashboard/Cases
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cases.ToListAsync());
        }

        // GET: Dashboard/Cases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @case = await _context.Cases
                .SingleOrDefaultAsync(m => m.Id == id);
            if (@case == null)
            {
                return NotFound();
            }

            return View(@case);
        }

        // GET: Dashboard/Cases/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Customers = await _context.Customers
                .Select(c => new SelectListItem {Value = c.Id.ToString(), Text = c.Name})
                .ToListAsync();
            return View();
        }

        // POST: Dashboard/Cases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ObjectResult> Create([Bind("Title,CustomerId")] Case @case)
        {
            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            @case.Date = DateTime.UtcNow;
            @case.LastModified = DateTime.UtcNow;
            _context.Cases.Add(@case);
            try
            {
                await _context.SaveChangesAsync();
                return new ObjectResult(@case.Id);
            }
            catch (DbUpdateException)
            {
                return new ObjectResult(null)
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                };
            }
        }

        // GET: Dashboard/Cases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @case = await _context.Cases.Include(c => c.Blocks).SingleOrDefaultAsync(m => m.Id == id);
            if (@case == null)
            {
                return NotFound();
            }

            ViewBag.Customers = await _context.Customers
                .Select(c => new SelectListItem {Value = c.Id.ToString(), Text = c.Name})
                .ToListAsync();
            return View(@case);
        }

        // POST: Dashboard/Cases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ObjectResult> Edit(int id, [Bind("Id,CustomerId,Title,Date")] Case @case)
        {
            if (id != @case.Id)
            {
                return new NotFoundObjectResult(null);
            }

            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            try
            {
                @case.LastModified = DateTime.UtcNow;
                _context.Update(@case);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CaseExists(@case.Id))
                {
                    return new NotFoundObjectResult(null);
                }

                throw;
            }

            return new ObjectResult(@case.Id);
        }

        // GET: Dashboard/Cases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @case = await _context.Cases
                .SingleOrDefaultAsync(m => m.Id == id);
            if (@case == null)
            {
                return NotFound();
            }

            return View(@case);
        }

        // POST: Dashboard/Cases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @case = await _context.Cases.Include(c => c.Blocks).SingleOrDefaultAsync(m => m.Id == id);
            _context.Blocks.RemoveRange(@case.Blocks);
            _context.Cases.Remove(@case);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CaseExists(int id)
        {
            return _context.Cases.Any(e => e.Id == id);
        }
    }
}
