using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class CasesController : Controller
    {
        private readonly ICaseRepository _caseRepository;
        private readonly IUploadService _uploadService;

        public CasesController(ICaseRepository caseRepository, IUploadService uploadService)
        {
            _caseRepository = caseRepository;
            _uploadService = uploadService;
        }

        // GET: Dashboard/Cases
        public async Task<IActionResult> Index(bool showDisabled = false)
        {
            ViewBag.ShowDisabled = showDisabled;
            return View(await _caseRepository.GetAll().Where(c => !c.Deleted && (c.Enabled || showDisabled))
                .OrderByDescending(c => c.Date).ToListAsync());
        }

        // GET: Dashboard/Cases/Deleted
        public async Task<IActionResult> Deleted()
        {
            return View(await _caseRepository.GetAll().Where(c => c.Deleted).OrderByDescending(c => c.Date).ToListAsync());
        }

        // GET: Dashboard/Cases/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Cases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ObjectResult> Create([Bind("Title,CustomerId,Image,TagIds,SharingDescription")] Case @case)
        {
            if (@case.Image == null)
                ModelState.AddModelError(nameof(@case.Image), "The Image field is required.");
            else
            {
                if (!(new[] {".png", ".jpg", ".jpeg"}).Contains(Path.GetExtension(@case.Image.FileName)?.ToLower()))
                    ModelState.AddModelError(nameof(@case.Image), "Invalid image type, only png and jpg images are allowed");

                if (@case.Image.Length < 1)
                    ModelState.AddModelError(nameof(@case.Image), "Filesize too small");
            }

            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            if (@case.Image != null)
            {
                @case.PhotoPath = await _uploadService.Upload(@case.Image, "/images/uploads/cases");
            }

            @case.CaseTags = @case.TagIds?.Select(tagId => new CaseTag {Case = @case, TagId = tagId}).ToList();
            @case.Language = "nl";
            @case.Date = DateTime.UtcNow;
            @case.LastModified = DateTime.UtcNow;
            try
            {
                await _caseRepository.Create(@case);
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
            var @case = await _caseRepository.Get(id ?? 0, true);
            if (@case == null)
            {
                return NotFound();
            }

            return View(@case);
        }

        // POST: Dashboard/Cases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ObjectResult> EditPost(int? id)
        {
            var @case = await _caseRepository.Get(id ?? 0, true);

            if (@case == null) return new NotFoundObjectResult(null);

            // Bind POST variables Title, CustomerId, Image and TagIds to the model.
            await TryUpdateModelAsync(@case, string.Empty, c => c.Title, c => c.CustomerId, c => c.Image, c => c.TagIds, c => c.SharingDescription);

            if (@case.Image != null)
            {
                if (!(new[] {".png", ".jpg", ".jpeg"}).Contains(Path.GetExtension(@case.Image.FileName)?.ToLower()))
                    ModelState.AddModelError(nameof(@case.Image), "Invalid image type, only png and jpg images are allowed");

                if (@case.Image.Length == 0)
                    ModelState.AddModelError(nameof(@case.Image), "Filesize too small");
            }

            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            if (@case.Image != null)
            {
                if (@case.PhotoPath != null)
                    await _uploadService.Delete(@case.PhotoPath);
                @case.PhotoPath = await _uploadService.Upload(@case.Image, "/images/uploads/cases");
            }

            @case.CaseTags.RemoveAll(ct => !(@case.TagIds?.Contains(ct.TagId) ?? false));
            @case.CaseTags.AddRange(@case.TagIds?.Except(@case.CaseTags.Select(ct => ct.TagId))
                .Select(tagId => new CaseTag {Case = @case, TagId = tagId}) ?? new List<CaseTag>());

            try
            {
                @case.LastModified = DateTime.UtcNow;
                await _caseRepository.Update(@case);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CaseExists(@case.Id))
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
            var @case = await _caseRepository.Get(id ?? 0, true);
            if (@case == null)
            {
                return NotFound();
            }

            return View(@case);
        }

        // POST: Dashboard/Cases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var @case = await _caseRepository.Get(id ?? 0, true);
            if (@case == null)
            {
                return NotFound();
            }

            @case.Deleted = true;

            await _caseRepository.Update(@case);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Restore(int? id)
        {
            var @case = await _caseRepository.Get(id ?? 0);
            if (@case == null)
            {
                return NotFound();
            }

            @case.Deleted = false;

            await _caseRepository.Update(@case);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleEnable(int? id)
        {
            var @case = await _caseRepository.Get(id ?? 0);
            if (@case == null)
            {
                return NotFound();
            }

            @case.Enabled  = !@case.Enabled;

            await _caseRepository.Update(@case);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CaseExists(int id)
        {
            return (await _caseRepository.Get(id)) != null;
        }
    }
}
