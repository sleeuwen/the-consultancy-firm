using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class TagRepository : ITagRepository
    {
        private ApplicationDbContext _context;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tag> Get(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public IQueryable<Tag> GetAll()
        {
            return _context.Tags;
        }

        public async Task<IEnumerable<Tag>> Search(string term)
        {
            if (term == null || term.Trim() == "")
                return await GetAll().ToListAsync();

            var q = term.Trim().ToLower();
            return await _context.Tags.Where(t => t.Text.ToLower().Contains(q)).ToListAsync();
        }

        public async Task Create(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var tag = await Get(id);
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }
}
