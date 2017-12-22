using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class BlocksController : Controller
    {
        private readonly ICaseRepository _caseRepository;
        private readonly IBlockRepository _blockRepository;

        public BlocksController(IBlockRepository blockRepository, ICaseRepository caseRepository)
        {
            _blockRepository = blockRepository;
            _caseRepository = caseRepository;
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

        private void SetTypeId(Block block, Enumeration.ContentItemType type, int id)
        {
            switch (type)
            {
                case Enumeration.ContentItemType.Case:
                    block.CaseId = id;
                    break;
                case Enumeration.ContentItemType.Download:
                    block.DownloadId = id;
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
