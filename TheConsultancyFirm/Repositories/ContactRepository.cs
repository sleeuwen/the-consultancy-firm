using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TheConsultancyFirm.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Contact> Get(int id)
        {
            return _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Contact>> GetAll()
        {
            return await _context.Contacts.ToListAsync();
        }

        public Task AddAsync(Contact contact)
        {
            _context.Add(contact);
            return _context.SaveChangesAsync();
        }

        public int CountUnread()
        {
            return _context.Contacts.Count(c => c.Read == false);
        }

        public Task Update(Contact contact)
        {
            _context.Update(contact);
            return _context.SaveChangesAsync();
        }
    }
}
