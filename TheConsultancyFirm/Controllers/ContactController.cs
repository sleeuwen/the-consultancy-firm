using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;

namespace TheConsultancyFirm.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactRepository _repository;
        private readonly IMailService _mailService;

        public ContactController(IContactRepository repository, IMailService mailService)
        {
            _repository = repository;
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
            await _repository.AddAsync(contact);

            // Send notification email
            _mailService.SendContactMailAsync(contact);

            TempData["ContactMessage"] = "Je bericht is verzonden.";
            return RedirectToAction(nameof(Index), null, "contact");
        }
    }
}
