using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class CasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICaseRepository _caseRepository;
        private readonly IUploadService _uploadService;

        public CasesController(ApplicationDbContext context, ICaseRepository caseRepository, IUploadService uploadService)
        {
            _context = context;
            _caseRepository = caseRepository;
            _uploadService = uploadService;
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
        public async Task<ObjectResult> Create([Bind("Title,CustomerId,Image,TagIds")]
            Case @case)
        {
            if (@case.Image != null && @case.Image?.Length == 0)
                ModelState.AddModelError(nameof(@case.Image), "Filesize too small");

            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            if (@case.Image != null)
            {
                var extension = Path.GetExtension(@case.Image.FileName);
                if (extension != ".jpg" && extension != ".png" && extension != ".jpeg")
                {
                    ModelState.AddModelError(nameof(@case.Image), "The uploaded file was not an image.");
                    return new BadRequestObjectResult(ModelState);
                }

                @case.PhotoPath = await _uploadService.Upload(@case.Image, "/images/cases");
            }

            @case.CaseTags = @case.TagIds.Select(tagId => new CaseTag {Case = @case, TagId = tagId}).ToList();

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
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ObjectResult> EditPost(int? id)
        {
            var @case = await _caseRepository.Get(id ?? 0);

            if (@case == null) return new NotFoundObjectResult(null);

            // Bind POST variables Title, CustomerId, Image and TagIds to the model.
            await TryUpdateModelAsync(@case, string.Empty, c => c.Title, c => c.CustomerId, c => c.Image, c => c.TagIds);

            if (@case.Image != null && @case.Image?.Length == 0)
                ModelState.AddModelError(nameof(@case.Image), "Filesize too small");

            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            if (@case.Image != null)
            {
                var extension = Path.GetExtension(@case.Image.FileName);
                if (extension != ".jpg" && extension != ".png" && extension != ".jpeg")
                {
                    ModelState.AddModelError(nameof(@case.Image), "The uploaded file was not an image.");
                    return new BadRequestObjectResult(ModelState);
                }

                await _uploadService.Delete(@case.PhotoPath);
                @case.PhotoPath = await _uploadService.Upload(@case.Image, "/images/cases");
            }

            @case.CaseTags.RemoveAll(ct => !(@case.TagIds?.Contains(ct.TagId) ?? false));
            @case.CaseTags.AddRange(@case.TagIds?.Except(@case.CaseTags.Select(ct => ct.TagId))
                .Select(tagId => new CaseTag {Case = @case, TagId = tagId}) ?? new List<CaseTag>());

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
