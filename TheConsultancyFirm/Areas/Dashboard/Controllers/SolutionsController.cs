using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class SolutionsController : Controller
    {
        private readonly ISolutionRepository _solutionRepository;
        private readonly IUploadService _uploadService;

        public SolutionsController(ISolutionRepository solutionRepository, IUploadService uploadService)
        {
            _solutionRepository = solutionRepository;
            _uploadService = uploadService;
        }

        // GET: Dashboard/Solutions
        public async Task<IActionResult> Index()
        {
            return View(_solutionRepository.GetAll());
        }

        // GET: Dashboard/Solutions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solution = await _solutionRepository.Get((int)id,true);

            if (solution == null)
            {
                return NotFound();
            }

            return View(solution);
        }

        // GET: Dashboard/Solutions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Solutions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ObjectResult> Create([Bind("Title,Image, CustomerIds, TagIds, SharingDescription")] Solution solution)
        {
            if (solution.Image == null)
                ModelState.AddModelError(nameof(solution.Image), "The Image field is required.");
            else
            {
                if (!(new[] {".png", ".jpg", ".jpeg"}).Contains(Path.GetExtension(solution.Image.FileName)?.ToLower()))
                    ModelState.AddModelError(nameof(solution.Image),
                        "Invalid image type, only png and jpg images are allowed");

                if (solution.Image.Length < 1)
                    ModelState.AddModelError(nameof(solution.Image), "Filesize too small");
            }

            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            if (solution.Image != null)
            {
                solution.PhotoPath = await _uploadService.Upload(solution.Image, "/images/uploads/solutions");
            }

            solution.SolutionTags = solution.TagIds?.Select(tagId => new SolutionTag { Solution = solution, TagId = tagId }).ToList();
            solution.CustomerSolutions = solution.CustomerIds?.Select(customerId =>
                new CustomerSolution {CustomerId = customerId, Solution = solution}).ToList();

            solution.LastModified = DateTime.UtcNow;
            solution.Date = DateTime.UtcNow;

            try
            {
                await _solutionRepository.Create(solution);
                return new ObjectResult(solution.Id);
            }
            catch (DbUpdateException)
            {
                return new ObjectResult(null)
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                };
            }
        }

        // GET: Dashboard/Solutions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solution = await _solutionRepository.Get((int) id, true);

            if (solution == null)
            {
                return NotFound();
            }
            return View(solution);
        }

        // POST: Dashboard/Solutions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            var solution = await _solutionRepository.Get(id ?? 0, true);

            if (solution == null) return new NotFoundObjectResult(null);

            // Bind POST variables Title, CustomerId, Image and TagIds to the model.
            await TryUpdateModelAsync(solution, string.Empty, s => s.Title, s => s.CustomerIds, s => s.Image, s => s.TagIds, c => c.SharingDescription);

            if (solution.Image != null)
            {
                if (!(new[] { ".png", ".jpg", ".jpeg" }).Contains(Path.GetExtension(solution.Image.FileName)?.ToLower()))
                    ModelState.AddModelError(nameof(solution.Image), "Invalid image type, only png and jpg images are allowed");

                if (solution.Image.Length == 0)
                    ModelState.AddModelError(nameof(solution.Image), "Filesize too small");
            }

            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            if (solution.Image != null)
            {
                if (solution.PhotoPath != null)
                    await _uploadService.Delete(solution.PhotoPath);
                solution.PhotoPath = await _uploadService.Upload(solution.Image, "/images/uploads/solutions");
            }

            solution.SolutionTags.RemoveAll(ct => !(solution.TagIds?.Contains(ct.TagId) ?? false));
            solution.SolutionTags.AddRange(solution.TagIds?.Except(solution.SolutionTags.Select(ct => ct.TagId))
                                        .Select(tagId => new SolutionTag { Solution = solution, TagId = tagId }) ?? new List<SolutionTag>());

            solution.CustomerSolutions.RemoveAll(ct => !(solution.CustomerIds?.Contains(ct.CustomerId) ?? false));
            solution.CustomerSolutions.AddRange(solution.CustomerIds?.Except(solution.CustomerSolutions.Select(ct => ct.CustomerId))
                                               .Select(customerId => new CustomerSolution { Solution = solution, CustomerId = customerId }) ?? new List<CustomerSolution>());

            try
            {
                solution.LastModified = DateTime.UtcNow;
                await _solutionRepository.Update(solution);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SolutionExists(solution.Id))
                {
                    return new NotFoundObjectResult(null);
                }

                throw;
            }

            return new ObjectResult(solution.Id);
        }

        // GET: Dashboard/Solutions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var solution = await _solutionRepository.Get((int)id, true);
            if (solution == null)
            {
                return NotFound();
            }

            return View(solution);
        }

        // POST: Dashboard/Solutions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var solution = await _solutionRepository.Get((int)id, true);
            await _solutionRepository.Delete(solution);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SolutionExists(int id)
        {
            return await _solutionRepository.Get(id, true) == null;
        }
    }
}
