using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Services;

namespace TheConsultancyFirm.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMailService _mailService;

        public ContactController(ApplicationDbContext context, IMailService mailService)
        {
            _context = context;
            _mailService = mailService;
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

            // Save to database
            _context.Add(contact);
            await _context.SaveChangesAsync();

            // Send notification email
            _mailService.SendContactMailAsync(contact);

            TempData["ContactMessage"] = "Je bericht is verzonden.";
            return Redirect(Url.Action(nameof(Index)) + "#contact");
        }
    }
}
