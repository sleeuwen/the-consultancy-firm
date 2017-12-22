using System.Threading.Tasks;
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
            var block = await _context.Blocks.FindAsync(id);
            _context.Remove(block);
            await _context.SaveChangesAsync();
        }
    }
}
