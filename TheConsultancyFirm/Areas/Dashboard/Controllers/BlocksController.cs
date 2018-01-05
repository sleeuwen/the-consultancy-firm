using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class BlocksController : Controller
    {
        private readonly IBlockRepository _blockRepository;
        private readonly IUploadService _uploadService;

        public BlocksController(IBlockRepository blockRepository, IUploadService uploadService)
        {
            _blockRepository = blockRepository;
            _uploadService = uploadService;
        }

        [HttpGet]
        public async Task<IActionResult> Carousel(Enumeration.ContentItemType contentType, int contentId)
        {
            var block = new CarouselBlock();
            SetTypeId(block, contentType, contentId);
            block.LastModified = block.Date = DateTime.UtcNow;
            await _blockRepository.Create(block);

            return ViewComponent("Block", block);
        }

        [HttpGet]
        public async Task<IActionResult> Quote(Enumeration.ContentItemType contentType, int contentId)
        {
            var block = new QuoteBlock();
            SetTypeId(block, contentType, contentId);
            block.LastModified = block.Date = DateTime.UtcNow;
            await _blockRepository.Create(block);

            return ViewComponent("Block", block);
        }

        [HttpGet]
        public async Task<IActionResult> SolutionAdvantages(Enumeration.ContentItemType contentType, int contentId)
        {
            var block = new SolutionAdvantagesBlock();
            SetTypeId(block, contentType, contentId);
            block.LastModified = block.Date = DateTime.UtcNow;
            await _blockRepository.Create(block);

            return ViewComponent("Block", block);
        }

        [HttpGet]
        public async Task<IActionResult> Text(Enumeration.ContentItemType contentType, int contentId)
        {
            var block = new TextBlock();
            SetTypeId(block, contentType, contentId);
            block.LastModified = block.Date = DateTime.UtcNow;
            await _blockRepository.Create(block);

            return ViewComponent("Block", block);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ObjectResult> Carousel(Enumeration.ContentItemType contentType, int contentId, int id, List<Slide> slides)
        {
            var block = await _blockRepository.Get(id);
            if (!(block is CarouselBlock carousel)) return new NotFoundObjectResult(null);

            List<string> photoPaths = carousel.Slides.Select(s => s.PhotoPath).ToList();

            await TryUpdateModelAsync(carousel, string.Empty, c => c.Order, c => c.LinkText, c => c.LinkPath);
            UpdateCarouselSlides(carousel, slides);

            for (var i = 0; i < slides.Count; i++)
                ValidateImageExtension(slides[i].Image, $"Slides[{i}].Image");

            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            foreach (var slide in carousel.Slides)
            {
                if (slide.Image != null)
                {
                    if (slide.PhotoPath != null) await _uploadService.Delete(slide.PhotoPath);
                    slide.PhotoPath = await _uploadService.Upload(slide.Image, "/images/uploads");
                }
            }

            SetTypeId(block, contentType, contentId);
            carousel.LastModified = DateTime.UtcNow;
            await _blockRepository.Update(carousel);

            var newPaths = carousel.Slides.Select(s => s.PhotoPath).ToList();
            foreach (var photoPath in photoPaths)
            {
                if (!newPaths.Contains(photoPath))
                {
                    await _uploadService.Delete(photoPath);
                }
            }

            return new OkObjectResult(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ObjectResult> Quote(Enumeration.ContentItemType contentType, int contentId, int id)
        {
            var block = await _blockRepository.Get(id);
            if (!(block is QuoteBlock quote)) return new NotFoundObjectResult(null);

            await TryUpdateModelAsync(quote, string.Empty, q => q.Order, q => q.Author, q => q.Text);
            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            SetTypeId(block, contentType, contentId);
            block.LastModified = DateTime.UtcNow;
            await _blockRepository.Update(block);

            return new OkObjectResult(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ObjectResult> SolutionAdvantages(Enumeration.ContentItemType contentType, int contentId, int id)
        {
            var block = await _blockRepository.Get(id);
            if (!(block is SolutionAdvantagesBlock advantages)) return new NotFoundObjectResult(null);

            await TryUpdateModelAsync(advantages, string.Empty, s => s.Order, s => s.Image, s => s.Advantages);
            ValidateImageExtension(advantages.Image, nameof(advantages.Image));
            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            if (advantages.Image != null)
            {
                if (advantages.PhotoPath != null) await _uploadService.Delete(advantages.PhotoPath);
                advantages.PhotoPath = await _uploadService.Upload(advantages.Image, "/images/uploads");
            }

            SetTypeId(block, contentType, contentId);
            advantages.LastModified = DateTime.UtcNow;
            await _blockRepository.Update(advantages);

            return new OkObjectResult(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ObjectResult> Text(Enumeration.ContentItemType contentType, int contentId, int id)
        {
            var block = await _blockRepository.Get(id);
            if (!(block is TextBlock text)) return new NotFoundObjectResult(null);

            await TryUpdateModelAsync(text, string.Empty, t => t.Order, t => t.Text);
            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            SetTypeId(block, contentType, contentId);
            block.LastModified = DateTime.UtcNow;
            await _blockRepository.Update(block);

            return new OkObjectResult(null);
        }

        [HttpDelete]
        public async Task Delete(int id)
        {
            var block = await _blockRepository.Get(id);

            if (block is CarouselBlock carousel)
            {
                foreach (var slide in carousel.Slides)
                {
                    if (slide.PhotoPath != null) await _uploadService.Delete(slide.PhotoPath);
                }
            }

            if (block is SolutionAdvantagesBlock advantages)
            {
                if (advantages.PhotoPath != null) await _uploadService.Delete(advantages.PhotoPath);
            }

            await _blockRepository.Delete(id);
        }

        [Route("/api/dashboard/blocks/upload")]
        [HttpPost]
        public async Task<ObjectResult> Upload(IFormFile file)
        {
            // File not sent
            if (file == null || file.Length == 0)
                return new BadRequestObjectResult((object) null);

            // Only accept jpg or png images
            var ext = Path.GetExtension(file.FileName);
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                return new BadRequestObjectResult((object) null);

            var filepath = await _uploadService.Upload(file, "/images/uploads");

            return new ObjectResult(new {location = filepath});
        }

        private void SetTypeId(Block block, Enumeration.ContentItemType type, int id)
        {
            switch (type)
            {
                case Enumeration.ContentItemType.Case:
                    block.CaseId = id;
                    break;
                case Enumeration.ContentItemType.News:
                    block.NewsItemId = id;
                    break;
                case Enumeration.ContentItemType.Solution:
                    block.SolutionId = id;
                    break;
            }
        }

        private void UpdateCarouselSlides(CarouselBlock carousel, List<Slide> slides)
        {
            if (slides == null)
            {
                carousel.Slides.RemoveAll(s => true);
                return;
            }

            slides = slides.OrderBy(s => s.Order).ToList();
            int i;

            for (i = 0; i < slides.Count; i++)
            {
                if (carousel.Slides.Count == i)
                {
                    carousel.Slides.Add(new Slide
                    {
                        Text = slides[i].Text,
                        Image = slides[i].Image,
                        Order = i
                    });
                }
                else
                {
                    carousel.Slides[i].Text = slides[i].Text;
                    carousel.Slides[i].Image = slides[i].Image;
                    carousel.Slides[i].PhotoPath = slides[i].PhotoPath;
                    carousel.Slides[i].Order = i;
                }
            }

            carousel.Slides.RemoveRange(i, carousel.Slides.Count - i);
        }

        private void ValidateImageExtension(IFormFile image, string key)
        {
            var extensions = new[] {".png", ".jpg", ".jpeg"};
            if (image != null && !extensions.Contains(Path.GetExtension(image.FileName)))
                ModelState.AddModelError(key, "Invalid image type, only png and jpg images are allowed");
        }
    }
}
