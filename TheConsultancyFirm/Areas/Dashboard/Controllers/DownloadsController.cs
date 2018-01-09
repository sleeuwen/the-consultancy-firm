using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class DownloadsController : Controller
    {
        private readonly IDownloadRepository _downloadRepository;
        private readonly IUploadService _uploadService;

        public DownloadsController(IDownloadRepository downloadRepository, IUploadService uploadService)
        {
            _downloadRepository = downloadRepository;
            _uploadService = uploadService;
        }

        // GET: Downloads
        public async Task<IActionResult> Index(bool showDisabled = false)
        {
            if (showDisabled)
            {
                return View(await _downloadRepository.GetAll().OrderByDescending(d => d.Date).ToListAsync());
            }
            return View(await _downloadRepository.GetAll().Where(n => n.Enabled).OrderByDescending(d => d.Date).ToListAsync());
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

            download.Enabled = false;

            await _downloadRepository.Update(download);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> DownloadExists(int id)
        {
            return await _downloadRepository.Get(id) != null;
        }
    }
}
