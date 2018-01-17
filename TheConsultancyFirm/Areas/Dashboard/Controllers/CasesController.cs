using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Areas.Dashboard.ViewModels;
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
        private readonly IItemTranslationRepository _itemTranslationRepository;

        public CasesController(ICaseRepository caseRepository, IUploadService uploadService, IItemTranslationRepository itemTranslationRepository)
        {
            _caseRepository = caseRepository;
            _uploadService = uploadService;
            _itemTranslationRepository = itemTranslationRepository;
        }

        // GET: Dashboard/Cases
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page,
            bool showDisabled = false)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["LastModifiedSortParm"] = sortOrder == "LastModified" ? "last_desc" : "LastModified";

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
            var cases =  _caseRepository.GetAll().Where(c => !c.Deleted && (c.Enabled || showDisabled) && (string.IsNullOrEmpty(searchString) || c.Title.Contains(searchString)));

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
                    cases = cases.OrderBy(c => c.LastModified);
                    break;
                case "last_desc":
                    cases = cases.OrderByDescending(c => c.LastModified);
                    break;
                default:
                    cases = cases.OrderBy(c => c.Title);
                    break;
            }
            return View(new CaseViewModel
            {
                CasesList = await PaginatedList<Case>.Create(cases, page ?? 1, 5),
                CasesWithoutTranslation = await _itemTranslationRepository.GetCasesWithoutTranslation()
            });

        }

        // GET: Dashboard/Cases/Deleted
        public async Task<IActionResult> Deleted()
        {
            return View(await _caseRepository.GetAll().Where(c => c.Deleted).OrderByDescending(c => c.Date).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> TranslationChoice(int choice, int selectBox = 0)
        {
            if (choice == 0) return RedirectToAction("Create");
            if (selectBox == 0) return NotFound();
            var id = await _caseRepository.CreateCopy(selectBox);
            return RedirectToAction("TranslationEdit", new {id = id});
        }

        public async Task<IActionResult> TranslationEdit(int id)
        {
            return View(await _caseRepository.Get(id));
        }

        [HttpPost, ActionName("SaveTranslation")]
        [ValidateAntiForgeryToken]
        public async Task<ObjectResult> EditTranslationPost(int? id)
        {
            var @case = await _caseRepository.Get(id ?? 0, true);

            if (@case == null) return new NotFoundObjectResult(null);

            // Bind POST variables Title, CustomerId, Image and TagIds to the model.
            await TryUpdateModelAsync(@case, string.Empty, c => c.Title, c => c.SharingDescription);

            var a = ModelState;
            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);
            
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
