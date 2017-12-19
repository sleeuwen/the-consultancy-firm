using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(new Contact());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Name,Email,Mobile,Subject,Message")] Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return View(contact);
            }

            _context.Add(contact);
            await _context.SaveChangesAsync();

            TempData["ContactMessage"] = "Je bericht is verzonden.";
            return Redirect(Url.Action(nameof(Index)) + "#contact");
        }
    }
}
