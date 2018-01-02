using System;
using System.IO;
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
        public async Task<IActionResult> Index()
        {
            return View(await _downloadRepository.GetAll().ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Title,Description,File,LinkPath")] Download download)
        {
            if (!ModelState.IsValid) return View(download);

            if (download.File?.Length > 0)
            {
                download.Date = DateTime.UtcNow;

                download.LinkPath = await _uploadService.Upload(download.File, "/files",
                    Path.GetFileNameWithoutExtension(download.File.FileName));
            }
            else
            {
                ModelState.AddModelError("File", "No File Chosen");
                return View(download);
            }

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Date,File,LinkPath")] Download download)
        {
            if (id != download.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) return View(download);

            if (download.File != null)
            {
                if (download.LinkPath != null)
                    await _uploadService.Delete(download.LinkPath);

                download.LinkPath = await _uploadService.Upload(download.File, "/files",
                    Path.GetFileNameWithoutExtension(download.File.FileName));
            }
            else if (string.IsNullOrWhiteSpace(download.LinkPath))
            {
                ModelState.AddModelError("File", "No File Chosen");
                return View(download);
            }

            try
            {
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
            if (download.LinkPath != null)
                await _uploadService.Delete(download.LinkPath);
            await _downloadRepository.Delete(id);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> DownloadExists(int id)
        {
            return await _downloadRepository.Get(id) != null;
        }
    }
}
