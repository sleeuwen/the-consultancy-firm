using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Services;
using Microsoft.AspNetCore.Identity;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMailService _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(ApplicationDbContext context, IMailService emailSender, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        // GET: Dashboard/Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApplicationUsers.ToListAsync());
        }

        // GET: Dashboard/Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Users/Create (Email + misschien password, of hier wordt al een mail voor opgestuurd)
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(applicationUser.Email))
            {
                string message;
                string password = GenerateRandonPassword();
                await _userManager.AddPasswordAsync(applicationUser,password);
                
                message = "Beste nieuwe gebruiker, <br/><br/>" +
                    "Er is een account aangemaakt voor je met dit email address. <br/>" +
                    "Er is ook een wachtwoord aangemaakt voor je en dat is: " + password + "<br/><br/>" +
                    "The Consultancy Firm";

                await _emailSender.SendMailAsync(applicationUser.Email, "Nieuw account aangemaakt", message);

                _context.Add(applicationUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(applicationUser);
        }

        /// <summary>
        /// Genereerd een random wachtwoord met minimaal een hoofletter, speciaal karakter en een cijfer.
        /// </summary>
        /// <returns></returns>
        private string GenerateRandonPassword()
        {
            return "Test123!";
        }

        // GET: Dashboard/Users/Delete/5 (via modal!)
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // POST: Dashboard/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _context.ApplicationUsers.SingleOrDefaultAsync(m => m.Id == id);
            _context.ApplicationUsers.Remove(applicationUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
