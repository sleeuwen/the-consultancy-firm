using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;
using System.Collections.Generic;

namespace TheConsultancyFirm.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Contact contact)
        {
            _context.Add(contact);
            return _context.SaveChangesAsync();
        }

        public int CountUnreaded()
        {
            return _context.Contacts.Count(c => c.Readed == false);
        }

        public IEnumerable<Contact> GetAll()
        {
            return _context.Contacts;
        }

        public Task Update(Contact contact)
        {
            _context.Update(contact);
            return _context.SaveChangesAsync();
        }
    }
}
