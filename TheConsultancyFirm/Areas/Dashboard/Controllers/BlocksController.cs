using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Extensions;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class BlocksController : Controller
    {
        private readonly ICaseRepository _caseRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly IUploadService _uploadService;

        public BlocksController(IBlockRepository blockRepository, ICaseRepository caseRepository, IUploadService uploadService)
        {
            _blockRepository = blockRepository;
            _caseRepository = caseRepository;
            _uploadService = uploadService;
        }

        public IActionResult Carousel()
        {
            return ViewComponent("Block", new CarouselBlock());
        }

        public IActionResult Quote()
        {
            return ViewComponent("Block", new QuoteBlock());
        }

        public IActionResult SolutionAdvantages()
        {
            return ViewComponent("Block", new SolutionAdvantagesBlock());
        }
        public IActionResult Text()
        {
            return ViewComponent("Block", new TextBlock());
        }

        [HttpPost]
        public async Task Quote(Enumeration.ContentItemType contentType, int contentId, [Bind("Id,Date,Order,Text,Author")] QuoteBlock block)
        {
            if (block.Id == 0) block.Date = DateTime.UtcNow;
            block.LastModified = DateTime.UtcNow;
            SetTypeId(block, contentType, contentId);
            await _blockRepository.Update(block);
        }

        [HttpPost]
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
    }
}
