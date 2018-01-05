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
            await _blockRepository.Create(block);

            return ViewComponent("Block", block);
        }

        [HttpGet]
        public async Task<IActionResult> Quote(Enumeration.ContentItemType contentType, int contentId)
        {
            var block = new QuoteBlock();
            SetTypeId(block, contentType, contentId);
            await _blockRepository.Create(block);

            return ViewComponent("Block", block);
        }

        [HttpGet]
        public async Task<IActionResult> SolutionAdvantages(Enumeration.ContentItemType contentType, int contentId)
        {
            var block = new SolutionAdvantagesBlock();
            SetTypeId(block, contentType, contentId);
            await _blockRepository.Create(block);

            return ViewComponent("Block", block);
        }

        [HttpGet]
        public async Task<IActionResult> Text(Enumeration.ContentItemType contentType, int contentId)
        {
            var block = new TextBlock();
            SetTypeId(block, contentType, contentId);
            await _blockRepository.Create(block);

            return ViewComponent("Block", block);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Carousel(Enumeration.ContentItemType contentType, int contentId, int id, List<Slide> slides)
        {
            var block = await _blockRepository.Get(id);
            if (!(block is CarouselBlock carousel)) return;

            List<string> photoPaths = carousel.Slides.Select(s => s.PhotoPath).ToList();

            await TryUpdateModelAsync(carousel, string.Empty, c => c.Order, c => c.LinkText, c => c.LinkPath);
            UpdateCarouselSlides(carousel, slides);

            foreach (var slide in carousel.Slides)
            {
                if (slide.Image != null)
                {
                    // Only accept jpg or png images
                    var ext = Path.GetExtension(slide.Image.FileName);
                    if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                        continue; // TODO: return error

                    if (slide.PhotoPath != null) await _uploadService.Delete(slide.PhotoPath);
                    slide.PhotoPath = await _uploadService.Upload(slide.Image, "/images/uploads");
                }
            }

            carousel.LastModified = DateTime.UtcNow;
            SetTypeId(carousel, contentType, contentId);
            await _blockRepository.Update(carousel);

            var newPaths = carousel.Slides.Select(s => s.PhotoPath).ToList();
            foreach (var photoPath in photoPaths)
            {
                if (!newPaths.Contains(photoPath))
                {
                    await _uploadService.Delete(photoPath);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Quote(Enumeration.ContentItemType contentType, int contentId, [Bind("Id,Date,Order,Text,Author")] QuoteBlock block)
        {
            if (block.Id == 0) block.Date = DateTime.UtcNow;
            block.LastModified = DateTime.UtcNow;
            SetTypeId(block, contentType, contentId);
            await _blockRepository.Update(block);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task SolutionAdvantages(Enumeration.ContentItemType contentType, int contentId, int id)
        {
            var block = await _blockRepository.Get(id);
            if (!(block is SolutionAdvantagesBlock solutionAdvantagesBlock)) return;

            await TryUpdateModelAsync(solutionAdvantagesBlock, string.Empty, s => s.Image, s => s.Advantages);

            if (solutionAdvantagesBlock.Image != null)
            {
                // Only accept jpg or png images
                var ext = Path.GetExtension(solutionAdvantagesBlock.Image.FileName);
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
                    return; // TODO: return error

                if (solutionAdvantagesBlock.PhotoPath != null)
                    await _uploadService.Delete(solutionAdvantagesBlock.PhotoPath);
                solutionAdvantagesBlock.PhotoPath =
                    await _uploadService.Upload(solutionAdvantagesBlock.Image, "/images/uploads");
            }

            solutionAdvantagesBlock.LastModified = DateTime.UtcNow;
            SetTypeId(solutionAdvantagesBlock, contentType, contentId);
            await _blockRepository.Update(solutionAdvantagesBlock);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Text(Enumeration.ContentItemType contentType, int contentId, [Bind("Id,Date,Order,Text")] TextBlock block)
        {
            if (block.Id == 0) block.Date = DateTime.UtcNow;
            block.LastModified = DateTime.UtcNow;
            SetTypeId(block, contentType, contentId);
            await _blockRepository.Update(block);
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
    }
}
