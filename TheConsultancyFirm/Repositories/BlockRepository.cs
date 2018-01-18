using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class BlockRepository : IBlockRepository
    {
        public ApplicationDbContext _context { get; set; }

        public BlockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Block> Get(int id)
        {
            var block = await _context.Blocks.Include(b => b.Solution).SingleOrDefaultAsync(b => b.Id == id);

            if (block is CarouselBlock carousel)
            {
                await _context.Entry(carousel)
                    .Collection(c => c.Slides)
                    .LoadAsync();
            }

            return block;
        }

        public async Task<CarouselBlock> GetHomepageCarousel()
        {
            var carousel = await _context.Blocks.OfType<CarouselBlock>()
                .Include(c => c.Slides)
                .Where(c => c.HomepageCarousel)
                .SingleOrDefaultAsync();

            if (carousel == null)
            {
                carousel = new CarouselBlock
                {
                    Active = true,
                    Date = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                    HomepageCarousel = true,
                };

                await Create(carousel);
            }

            return carousel;
        }

        public async Task Create(Block block)
        {
            _context.Add(block);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Block block)
        {
            _context.Update(block);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var block = await Get(id);
            if (block is CarouselBlock carousel)
            {
                foreach (var slide in carousel.Slides)
                {
                    _context.Remove(slide);
                }
            }
            _context.Remove(block);
            await _context.SaveChangesAsync();
        }
    }
}
