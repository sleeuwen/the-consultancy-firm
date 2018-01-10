using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Services;
using System.Linq;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailService _emailSender;

        public UsersController(UserManager<ApplicationUser> userManager, IMailService emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Dashboard/Users
        public async Task<IActionResult> Index(bool showDisabled = false)
        {
            ViewBag.ShowDisabled = showDisabled;
            if(showDisabled)
            {
                return View(await _userManager.Users.ToListAsync());
            }
            return View(await _userManager.Users.Where(u => u.Enabled).ToListAsync());
        }

        // GET: Dashboard/Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(applicationUser.Email))
            {
                var userPass = GenerateRandomPassword();
                applicationUser.UserName = applicationUser.Email;
                applicationUser.Enabled = true;
                await _userManager.CreateAsync(applicationUser);
                await _userManager.AddPasswordAsync(applicationUser, userPass);

                await _emailSender.SendMailAsync(applicationUser.Email, "Er is een account gecreëerd voor u op de website.", $@"
                Beste gebruiker,<br/>
                <br/>
                Er is een account voor u aangemaakt op de website.<br/>
                Het wachtwoord voor de eerste keer inloggen in de applicatie is: <b>{userPass}</b><br/>
                Als u voor de eerste keer inlogt, wordt u verwezen naar de wachtwoord veranderen pagina, waar u dan de optie heeft om uw wachtwoord aan te passen.<br/>
                <br/>
                Met vriendelijke groet,<br/>
                Het TCF-team");

                return RedirectToAction(nameof(Index));
            }
            return View(applicationUser);
        }

        // POST: Dashboard/Users/Delete/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _userManager.FindByIdAsync(id);
            applicationUser.Enabled = false;

             await _userManager.UpdateAsync(applicationUser);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAccount(string id)
        {
            var applicationUser = await _userManager.FindByIdAsync(id);
            applicationUser.Enabled = true;

            await _userManager.UpdateAsync(applicationUser);

            return RedirectToAction(nameof(Index));
        }

        private static string GenerateRandomPassword()
        {
            var randomPassword = "";
            for (var i = 0; i < 10; i++)
            {
                var num = 0;
                var rand = new Random();
                if (i == 1)
                {
                    randomPassword += rand.Next(0,9);
                }
                else if (i == 4)
                {
                    randomPassword += "?";
                }
                else if (i == 8)
                {
                    num = rand.Next(0, 26);
                    
                    randomPassword += Char.ToUpper((char)('a' + num));
                }
                else
                {
                    num = rand.Next(0, 26);
                    randomPassword += (char)('a' + num);
                }
            }
            return randomPassword;
        }
    }
}
