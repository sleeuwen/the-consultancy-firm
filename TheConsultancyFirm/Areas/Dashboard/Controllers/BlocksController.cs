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
        public async Task CreateQuote(Enumeration.ContentItemType contentType, int contentId, [Bind("Order,Text,Author")] QuoteBlock block)
        {
            block.Date = DateTime.UtcNow;
            block.LastModified = DateTime.UtcNow;

            SetTypeId(block, contentType, contentId);

            await _blockRepository.Create(block);
        }

        [HttpPost]
        public async Task CreateText(Enumeration.ContentItemType contentType, int contentId, [Bind("Order,Text")] TextBlock block)
        {
            block.Date = DateTime.UtcNow;
            block.LastModified = DateTime.UtcNow;

            SetTypeId(block, contentType, contentId);

            await _blockRepository.Create(block);
        }

        [HttpPost]
        public async Task<IActionResult> Quote(Enumeration.ContentItemType contentType, int contentId, [Bind("Id,Date,Order,Text,Author")] QuoteBlock quote)
        {
            var block = (QuoteBlock) await _blockRepository.Get(quote.Id);
            if (block == null)
            {
                block = new QuoteBlock {Date = DateTime.UtcNow};
                SetTypeId(block, contentType, contentId);
            }

            block.Author = quote.Author;
            block.Text = quote.Text;
            block.LastModified = DateTime.UtcNow;
            await _blockRepository.Update(block);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Text(Enumeration.ContentItemType contentType, int contentId, [Bind("Id,Date,Order,Text")] TextBlock text)
        {
            var block = (TextBlock) await _blockRepository.Get(text.Id);
            if (block == null)
            {
                block = new TextBlock {Date = DateTime.UtcNow};
                SetTypeId(block, contentType, contentId);
            }

            block.Text = text.Text;
            block.LastModified = DateTime.UtcNow;
            await _blockRepository.Update(block);

            return Ok();
        }

        [HttpPost]
        [HttpPost("{id}")]
        public async Task Save(int? id, BlockViewModel vm)
        {
            
        }

        [HttpDelete("{id}")]
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

    public class BlockViewModel
    {
        public int ContentId { get; set; }
        public Enumeration.ContentItemType ContentType { get; set; }

        public int Id { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
    }
}
