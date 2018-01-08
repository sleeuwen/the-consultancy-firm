using System;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
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
    public class SolutionsController : Controller
    {
        private readonly ISolutionRepository _solutionRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly IUploadService _uploadService;

        public SolutionsController(ISolutionRepository solutionRepository, IBlockRepository blockRepository, IUploadService uploadService)
        {
            _solutionRepository = solutionRepository;
            _blockRepository = blockRepository;
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
        public async Task<ObjectResult> Create([Bind("Id,Title,Image,Date,LastModified")] Solution solution)
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

            solution.LastModified = DateTime.UtcNow;
            solution.Date = DateTime.UtcNow;

            try
            {
                await _solutionRepository.Add(solution);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Date,LastModified")] Solution solution)
        {
            if (id != solution.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                solution.LastModified = DateTime.UtcNow;
                try
                {
                    await _solutionRepository.Update(solution);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SolutionExists(solution.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(solution);
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

        private bool SolutionExists(int id)
        {
            return _solutionRepository.Get(id, true) == null;
        }
    }
}
