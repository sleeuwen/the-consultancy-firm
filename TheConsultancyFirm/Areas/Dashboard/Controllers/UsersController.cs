using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Services;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailService _mailService;

        public UsersController(UserManager<ApplicationUser> userManager, IMailService mailService)
        {
            _userManager = userManager;
            _mailService = mailService;
        }

        // GET: Dashboard/Users
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page, bool showDisabled = false)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewBag.ShowDisabled = showDisabled;
            ApplicationUser currentUser = await GetCurrentUser();
            ViewBag.ShowDisabled = showDisabled;
            var users = _userManager.Users.Where(c => c.Id != currentUser.Id && (c.Enabled || showDisabled) && (string.IsNullOrEmpty(searchString) || c.Email.Contains(searchString))).ToList();

            switch (sortOrder)
            {
                case "name_desc":
                    users = users.OrderByDescending(c => c.Email).ToList();
                    break;
                case "Date":
                    users = users.OrderBy(c => c.LastLogin).ToList();
                    break;
                case "date_desc":
                    users = users.OrderByDescending(c => c.LastLogin).ToList();
                    break;
                default:
                    users = users.OrderBy(c => c.Email).ToList();
                    break;
            }
            return View(PaginatedList<ApplicationUser>.Create(users.AsQueryable(), page ?? 1, 2));
        }

        [HttpGet]
        public async Task<ApplicationUser> GetCurrentUser()
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);
            return currentUser;
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
            if (!ModelState.IsValid || string.IsNullOrEmpty(applicationUser.Email))
                return View(applicationUser);

            var userPass = GenerateRandomPassword();
            applicationUser.UserName = applicationUser.Email;
            applicationUser.Enabled = true;
            await _userManager.CreateAsync(applicationUser);
            await _userManager.AddPasswordAsync(applicationUser, userPass);

            var _ = _mailService.SendAccountCreatedMailAsync(applicationUser.Email, userPass);

            return RedirectToAction(nameof(Index));

        }

        // POST: Dashboard/Users/Delete/5
        /// <summary>
        /// Toggles between an enabled user and disabled user
        /// </summary>
        /// <param name="id">The user id (in this case an string)</param>
        /// <returns>Redirect to index</returns>
        [HttpPost, ActionName("ToggleAccount")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAccount(string id)
        {
            var applicationUser = await _userManager.FindByIdAsync(id);
            applicationUser.Enabled = !applicationUser.Enabled;

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
