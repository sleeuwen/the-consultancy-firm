using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class NewslettersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INewsletterSubscriptionRepository _newsletterSubscriptionRepository;
        private readonly IHostingEnvironment _environment;
        private readonly IMailService _mailService;

        public NewslettersController(ApplicationDbContext context, IMailService mailService, INewsletterSubscriptionRepository newsletterSubscriptionRepository,
            IHostingEnvironment environment)
        {
            _context = context;
            _mailService = mailService;
            _newsletterSubscriptionRepository = newsletterSubscriptionRepository;
            _environment = environment;
        }

        // GET: Dashboard/Newsletter
        public async Task<IActionResult> Index()
        {
            return View(await _context.NewsLetters.ToListAsync());
        }

        // GET: Dashboard/Newsletter/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsletter = await _context.NewsLetters
                .SingleOrDefaultAsync(m => m.Id == id);
            if (newsletter == null)
            {
                return NotFound();
            }

            return View(newsletter);
        }

        // GET: Dashboard/Newsletter/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Newsletter/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content")] Newsletter newsletter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(newsletter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newsletter);
        }

        // GET: Dashboard/Newsletter/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsletter = await _context.NewsLetters.SingleOrDefaultAsync(m => m.Id == id);
            if (newsletter == null)
            {
                return NotFound();
            }
            return View(newsletter);
        }

        // POST: Dashboard/Newsletter/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content")] Newsletter newsletter)
        {
            if (id != newsletter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(newsletter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsletterExists(newsletter.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(newsletter);
        }

        // GET: Dashboard/Newsletter/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsletter = await _context.NewsLetters
                .SingleOrDefaultAsync(m => m.Id == id);
            if (newsletter == null)
            {
                return NotFound();
            }

            return View(newsletter);
        }

        // POST: Dashboard/Newsletter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var newsletter = await _context.NewsLetters.SingleOrDefaultAsync(m => m.Id == id);
            _context.NewsLetters.Remove(newsletter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsletterExists(int id)
        {
            return _context.NewsLetters.Any(e => e.Id == id);
        }

        [HttpPost]
        public  IActionResult Send(Newsletter newsletter)
        {
            SendMail(newsletter);
            return View();
        }

        public async Task SendMail(Newsletter newsletter)
        {
            try
            {
                var fuck = _newsletterSubscriptionRepository.GetAll();
                foreach (var receiver in fuck)
                {
                    var sbMail = new StringBuilder();
                    using (var sReader = new StreamReader(_environment.WebRootPath+"/MailTemplate.html"))
                    {
                        sbMail.Append(sReader.ReadToEnd());
                        sbMail.Replace("{subject}", newsletter.Subject);
                        sbMail.Replace("{week}", GetWeekOfYear(new DateTime()).ToString());
                        sbMail.Replace("{0}", newsletter.NewsletterIntroText);
                        sbMail.Replace("{1}", newsletter.NewsletterOtherNews);
                        sbMail.Replace("{year}", DateTime.Now.Year.ToString());
                        sbMail.Replace("{unsubscribe}", HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/newsletters/unsubscribe/" + receiver.EncodedMail);
                    }

                    await _mailService.SendMailAsync(receiver.Email, newsletter.Subject,
                        sbMail.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }

        public static int GetWeekOfYear(DateTime time)
        {
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);

            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) + 1;
        }

    }
}
