using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
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
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Berichten";
            return View(await _repository.GetAll().ToListAsync());
        }
        
        // GET: Dashboard/Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Berichten";
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _repository.GetAll().SingleOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
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
