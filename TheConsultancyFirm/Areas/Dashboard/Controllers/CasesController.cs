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
        private readonly ICaseRepository _caseRepository;
        private readonly BlockRepository _blockRepository;
        private readonly IUploadService _uploadService;

        public CasesController(ICaseRepository caseRepository, BlockRepository blockRepository, IUploadService uploadService)
        {
            _caseRepository = caseRepository;
            _blockRepository = blockRepository;
            _uploadService = uploadService;
        }

        // GET: Dashboard/Cases
        public async Task<IActionResult> Index()
        {
            return View(await _caseRepository.GetAll().ToListAsync());
        }

        // GET: Dashboard/Cases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var @case = await _caseRepository.Get(id ?? 0);
            if (@case == null)
            {
                return NotFound();
            }

            return View(@case);
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

                @case.PhotoPath = await _uploadService.Upload(@case.Image, "/images/uploads/cases");
            }

            @case.CaseTags = @case.TagIds.Select(tagId => new CaseTag {Case = @case, TagId = tagId}).ToList();

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
            var @case = await _caseRepository.Get(id ?? 0);
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
            var @case = await _caseRepository.Get(id ?? 0);
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
            var @case = await _caseRepository.Get(id ?? 0);
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

            await _caseRepository.Delete(@case.Id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CaseExists(int id)
        {
            return (await _caseRepository.Get(id)) != null;
        }
    }
}
