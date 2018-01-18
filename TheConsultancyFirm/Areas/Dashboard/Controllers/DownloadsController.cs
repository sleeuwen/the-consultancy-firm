using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Areas.Dashboard.ViewModels;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize]
    public class DownloadsController : Controller
    {
        private readonly IDownloadRepository _downloadRepository;
        private readonly IUploadService _uploadService;
        private readonly IItemTranslationRepository _itemTranslationRepository;

        public DownloadsController(IDownloadRepository downloadRepository, IUploadService uploadService, IItemTranslationRepository itemTranslationRepository)
        {
            _downloadRepository = downloadRepository;
            _uploadService = uploadService;
            _itemTranslationRepository = itemTranslationRepository;
        }

        // GET: Downloads
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
            ViewData["AmountSortParm"] = sortOrder == "amount" ? "amount_desc" : "amount";

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

            var downloads = _downloadRepository.GetAll().Where(d => !d.Deleted && (d.Enabled || showDisabled) && (string.IsNullOrEmpty(searchString) || d.Title.Contains(searchString)));

            switch (sortOrder)
            {
                case "name_desc":
                    downloads = downloads.OrderByDescending(c => c.Title);
                    break;
                case "Date":
                    downloads = downloads.OrderBy(c => c.Date);
                    break;
                case "date_desc":
                    downloads = downloads.OrderByDescending(c => c.Date);
                    break;
                case "amount":
                    downloads = downloads.OrderBy(c => c.AmountOfDownloads);
                    break;
                case "amount_desc":
                    downloads = downloads.OrderByDescending(c => c.AmountOfDownloads);
                    break;
                default:
                    downloads = downloads.OrderBy(c => c.Title);
                    break;
            }
            return View(new DownloadViewModel
            {
                DownloadsList = await PaginatedList<Download>.Create(downloads, page ?? 1, 5),
                DownloadsWithoutTranslation = await _itemTranslationRepository.GetDownloadsWithoutTranslation()
            });

        }

        // GET: Dashboard/Downloads/Deleted
        public async Task<IActionResult> Deleted()
        {
            return View(await _downloadRepository.GetAll().Where(d => d.Deleted).OrderByDescending(c => c.Date).ToListAsync());
        }

        // GET: Downloads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var download = await _downloadRepository.Get((int) id);
            if (download == null)
            {
                return NotFound();
            }

            return View(download);
        }

        // GET: Downloads/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Downloads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,File,LinkPath,TagIds")] Download download)
        {
            if (download.File == null)
                ModelState.AddModelError(nameof(download.File), "The File field is required.");
            else if (download.File.Length < 1)
                ModelState.AddModelError(nameof(download.File), "Filesize too small");

            if (!ModelState.IsValid) return View(download);

            if (download.File != null)
            {
                download.LinkPath = await _uploadService.Upload(download.File, "/files",
                    Path.GetFileNameWithoutExtension(download.File.FileName));
            }

            download.DownloadTags = download.TagIds
                ?.Select(tagId => new DownloadTag {Download = download, TagId = tagId}).ToList();
            download.Language = "nl";
            download.LastModified = download.Date = DateTime.UtcNow;
            await _downloadRepository.Create(download);
            return RedirectToAction(nameof(Index));
        }

        // GET: Downloads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var download = await _downloadRepository.Get((int) id);
            if (download == null)
            {
                return NotFound();
            }

            return View(download);
        }

        // POST: Downloads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            var download = await _downloadRepository.Get(id ?? 0);
            if (download == null) return NotFound();

            await TryUpdateModelAsync(download, string.Empty, d => d.Title, d => d.Description, d => d.File,
                d => d.TagIds);

            if (download.File != null && download.File.Length < 1)
                ModelState.AddModelError(nameof(download.File), "Filesize too small");

            if (!ModelState.IsValid) return View(download);

            if (download.File != null)
            {
                if (download.LinkPath != null)
                    await _uploadService.Delete(download.LinkPath);

                download.LinkPath = await _uploadService.Upload(download.File, "/files",
                    Path.GetFileNameWithoutExtension(download.File.FileName));
            }

            download.DownloadTags.RemoveAll(dt => !(download.TagIds?.Contains(dt.TagId) ?? false));
            download.DownloadTags.AddRange(
                download.TagIds?.Except(download.DownloadTags.Select(dt => dt.TagId))
                    .Select(tagId => new DownloadTag {Download = download, TagId = tagId}) ?? new List<DownloadTag>());

            try
            {
                download.LastModified = DateTime.UtcNow;
                await _downloadRepository.Update(download);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DownloadExists(download.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> TranslationChoice(int choice, int selectBox = 0)
        {
            if (choice == 0) return RedirectToAction("Create");
            if (selectBox == 0) return NotFound();
            var id = await _downloadRepository.CreateCopy(selectBox);
            return RedirectToAction("TranslationEdit", new { id = id });
        }

        public async Task<IActionResult> TranslationEdit(int id)
        {
            return View(await _downloadRepository.Get(id));
        }

        [HttpPost, ActionName("SaveTranslation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTranslationPost(int? id)
        {
            var download = await _downloadRepository.Get(id ?? 0);

            if (download == null) return new NotFoundObjectResult(null);

            // Bind POST variables Title, CustomerId, Image and TagIds to the model.
            await TryUpdateModelAsync(download, string.Empty, d => d.Title, d => d.Description);

            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            try
            {
                download.LastModified = DateTime.UtcNow;
                await _downloadRepository.Update(download);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DownloadExists(download.Id))
                {
                    return new NotFoundObjectResult(null);
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Downloads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var download = await _downloadRepository.Get((int) id);
            if (download == null)
            {
                return NotFound();
            }

            return View(download);
        }

        // POST: Downloads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var download = await _downloadRepository.Get(id);
            if (download == null)
            {
                return NotFound();
            }

            download.Deleted = true;

            await _downloadRepository.Update(download);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Restore(int? id)
        {
            var download = await _downloadRepository.Get(id ?? 0);
            if (download == null)
            {
                return NotFound();
            }

            download.Deleted = false;

            await _downloadRepository.Update(download);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleEnable(int? id)
        {
            var download = await _downloadRepository.Get(id ?? 0);
            if (download == null)
            {
                return NotFound();
            }

            download.Enabled = !download.Enabled;

            await _downloadRepository.Update(download);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> DownloadExists(int id)
        {
            return await _downloadRepository.Get(id) != null;
        }
    }
}
