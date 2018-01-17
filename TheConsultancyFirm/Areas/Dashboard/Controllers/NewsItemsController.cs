using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HeyRed.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class NewsItemsController : Controller
    {
        private readonly INewsItemRepository _newsItemRepository;
        private readonly IUploadService _uploadService;

        public NewsItemsController(INewsItemRepository newsItemRepository, IUploadService uploadService)
        {
            _newsItemRepository = newsItemRepository;
            _uploadService = uploadService;
        }

        // GET: Dashboard/NewsItems
        public async Task<IActionResult> Index(bool showDisabled = false)
        {
            ViewBag.ShowDisabled = showDisabled;
            return View(await _newsItemRepository.GetAll().Where(n => !n.Deleted && (n.Enabled || showDisabled))
                .OrderByDescending(n => n.Date).ToListAsync());
        }

        // GET: Dashboard/Downloads/Deleted
        public async Task<IActionResult> Deleted()
        {
            return View(await _newsItemRepository.GetAll().Where(n => n.Deleted).OrderByDescending(c => c.Date).ToListAsync());
        }

        // GET: Dashboard/NewsItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/NewsItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ObjectResult> Create([Bind("Title,Image,TagIds,SharingDescription")] NewsItem newsItem)
        {
            if (newsItem.Image != null)
            {
                using (var stream = newsItem.Image.OpenReadStream())
                {
                    if (!(new[] {"image/png", "image/jpeg"}).Contains(MimeGuesser.GuessMimeType(stream)))
                        ModelState.AddModelError(nameof(newsItem.Image),
                            "Invalid image type, only png and jpg images are allowed");
                }

                if (newsItem.Image?.Length < 1)
                    ModelState.AddModelError(nameof(newsItem.Image), "Filesize too small");
            }

            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            if (newsItem.Image != null)
            {
                newsItem.PhotoPath = await _uploadService.Upload(newsItem.Image, "/images/uploads/newsitems");
            }

            newsItem.NewsItemTags = newsItem.TagIds?.Select(tagId => new NewsItemTag { NewsItem = newsItem, TagId = tagId }).ToList();

            newsItem.Date = DateTime.UtcNow;
            newsItem.LastModified = DateTime.UtcNow;
            try
            {
                await _newsItemRepository.Create(newsItem);
                return new ObjectResult(newsItem.Id);
            }
            catch (DbUpdateException)
            {
                return new ObjectResult(null)
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                };
            }
        }

        // GET: Dashboard/NewsItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var newsItem = await _newsItemRepository.Get(id ?? 0, true);
            if (newsItem == null)
            {
                return NotFound();
            }

            return View(newsItem);
        }

        // POST: Dashboard/NewsItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ObjectResult> EditPost(int? id)
        {
            var newsItem = await _newsItemRepository.Get(id ?? 0, true);

            if (newsItem == null) return new NotFoundObjectResult(null);

            // Bind POST variables Title, CustomerId, Image and TagIds to the model.
            await TryUpdateModelAsync(newsItem, string.Empty, c => c.Title, c => c.Image, c => c.TagIds, c => c.SharingDescription);

            if (newsItem.Image != null)
            {
                using (var stream = newsItem.Image.OpenReadStream())
                {
                    if (!(new[] {"image/png", "image/jpeg"}).Contains(MimeGuesser.GuessMimeType(stream)))
                        ModelState.AddModelError(nameof(newsItem.Image),
                            "Invalid image type, only png and jpg images are allowed");
                }

                if (newsItem.Image.Length == 0)
                    ModelState.AddModelError(nameof(newsItem.Image), "Filesize too small");
            }

            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            if (newsItem.Image != null)
            {
                if (newsItem.PhotoPath != null)
                    await _uploadService.Delete(newsItem.PhotoPath);
                newsItem.PhotoPath = await _uploadService.Upload(newsItem.Image, "/images/uploads/newsitems");
            }

            newsItem.NewsItemTags.RemoveAll(ct => !(newsItem.TagIds?.Contains(ct.TagId) ?? false));
            newsItem.NewsItemTags.AddRange(newsItem.TagIds?.Except(newsItem.NewsItemTags.Select(ct => ct.TagId))
                                        .Select(tagId => new NewsItemTag { NewsItem = newsItem, TagId = tagId }) ?? new List<NewsItemTag>());

            try
            {
                newsItem.LastModified = DateTime.UtcNow;
                await _newsItemRepository.Update(newsItem);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await NewsItemExists(newsItem.Id))
                {
                    return new NotFoundObjectResult(null);
                }

                throw;
            }

            return new ObjectResult(newsItem.Id);
        }

        // GET: Dashboard/NewsItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var newsItem = await _newsItemRepository.Get(id ?? 0, true);
            if (newsItem == null)
            {
                return NotFound();
            }

            return View(newsItem);
        }

        // POST: Dashboard/NewsItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var newsItem = await _newsItemRepository.Get(id ?? 0, true);
            if (newsItem == null)
            {
                return NotFound();
            }

            newsItem.Deleted = true;

            await _newsItemRepository.Update(newsItem);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Restore(int? id)
        {
            var newsItem = await _newsItemRepository.Get(id ?? 0);
            if (newsItem == null)
            {
                return NotFound();
            }

            newsItem.Deleted = false;

            await _newsItemRepository.Update(newsItem);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleEnable(int? id)
        {
            var newsItem = await _newsItemRepository.Get(id ?? 0);
            if (newsItem == null)
            {
                return NotFound();
            }

            newsItem.Enabled = !newsItem.Enabled;

            await _newsItemRepository.Update(newsItem);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> NewsItemExists(int id)
        {
            return (await _newsItemRepository.Get(id)) != null;
        }
    }
}
