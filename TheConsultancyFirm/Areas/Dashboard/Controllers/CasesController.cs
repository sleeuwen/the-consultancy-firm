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
using TheConsultancyFirm.Common;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class CasesController : Controller
    {
        private readonly ICaseRepository _caseRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly IUploadService _uploadService;

        public CasesController(ICaseRepository caseRepository, IBlockRepository blockRepository, IUploadService uploadService)
        {
            _caseRepository = caseRepository;
            _blockRepository = blockRepository;
            _uploadService = uploadService;
        }

        // GET: Dashboard/Cases
        public async Task<IActionResult> Index(
    string sortOrder,
    string currentFilter,
    string searchString,
    int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["LastModifiedSortParm"] = sortOrder == "LastModified" ? "date_desc" : "LastModified";



            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var cases = from c in _caseRepository.GetAll()
                        select c;
            if (!String.IsNullOrEmpty(searchString))
            {
                cases = cases.Where(c => c.Title.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    cases = cases.OrderByDescending(c => c.Title);
                    break;
                case "Date":
                    cases = cases.OrderBy(c => c.Date);
                    break;
                case "date_desc":
                    cases = cases.OrderByDescending(c => c.Date);
                    break;
                case "LastModified":
                    cases = cases.OrderByDescending(c => c.LastModified);
                    break;
                default:
                    cases = cases.OrderBy(c => c.Title);
                    break;
            }
            int pageSize = 3;
            return View(await PaginatedList<Case>.CreateAsync(cases.AsNoTracking(), page ?? 1, pageSize));

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
                if (!(new[] { ".png", ".jpg", ".jpeg" }).Contains(Path.GetExtension(@case.Image.FileName)?.ToLower()))
                    ModelState.AddModelError(nameof(@case.Image), "Invalid image type, only png and jpg images are allowed");

                if (@case.Image.Length < 1)
                    ModelState.AddModelError(nameof(@case.Image), "Filesize too small");
            }

            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            if (@case.Image != null)
            {
                @case.PhotoPath = await _uploadService.Upload(@case.Image, "/images/uploads/cases");
            }

            @case.CaseTags = @case.TagIds?.Select(tagId => new CaseTag { Case = @case, TagId = tagId }).ToList();

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
                if (!(new[] { ".png", ".jpg", ".jpeg" }).Contains(Path.GetExtension(@case.Image.FileName)?.ToLower()))
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
                .Select(tagId => new CaseTag { Case = @case, TagId = tagId }) ?? new List<CaseTag>());

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

            foreach (var block in @case.Blocks)
            {
                if (block is CarouselBlock carousel)
                {
                    foreach (var slide in carousel.Slides.Where(s => s.PhotoPath != null))
                    {
                        await _uploadService.Delete(slide.PhotoPath);
                    }
                }

                await _blockRepository.Delete(block.Id);
            }

            if (@case.PhotoPath != null)
                await _uploadService.Delete(@case.PhotoPath);

            await _caseRepository.Delete(@case.Id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CaseExists(int id)
        {
            return (await _caseRepository.Get(id)) != null;
        }
    }
}
