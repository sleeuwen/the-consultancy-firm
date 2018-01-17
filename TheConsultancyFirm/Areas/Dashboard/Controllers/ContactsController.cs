using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class ContactsController : Controller
    {
        private readonly IContactRepository _repository;

        public ContactsController(IContactRepository repository)
        {
            _repository = repository;
        }

        // GET: Dashboard/Contacts
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["SubjectSortParm"] = sortOrder == "Subject" ? "sub_desc" : "Subject";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            var contacts = (await _repository.GetAll()).ToList().Where(c => string.IsNullOrEmpty(searchString) || c.Email.Contains(searchString) || c.Name.Contains(searchString));

            switch (sortOrder)
            {
                case "name_desc":
                    contacts = contacts.OrderByDescending(c => c.Email).ToList();
                    break;
                case "Date":
                    contacts = contacts.OrderBy(c => c.Date).ToList();
                    break;
                case "name":
                    contacts = contacts.OrderBy(c => c.Email).ToList();
                    break;
                case "Subject":
                    contacts = contacts.OrderBy(c => c.Subject).ToList();
                    break;
                case "sub_desc":
                    contacts = contacts.OrderByDescending(c => c.Subject).ToList();
                    break;
                default:
                    contacts = contacts.OrderByDescending(c => c.Date).ToList();
                    break;
            }
            return View(await PaginatedList<Contact>.Create(contacts.AsQueryable(), page ?? 1, 5));
        }

        // GET: Dashboard/Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Berichten";
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _repository.Get((int) id);
            if (contact == null)
            {
                return NotFound();
            }

            if (!contact.Read)
            {
                contact.Read = true;
                await _repository.Update(contact);
            }

            return View(contact);
        }

        //// GET: Dashboard/Contacts/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var contact = await _context.Contacts
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (contact == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(contact);
        //}

        //// POST: Dashboard/Contacts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var contact = await _context.Contacts.SingleOrDefaultAsync(m => m.Id == id);
        //    _context.Contacts.Remove(contact);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ContactExists(int id)
        //{
        //    return _context.Contacts.Any(e => e.Id == id);
        //}
    }
}
