using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUploadService _uploadService;

        public CustomersController(ICustomerRepository customerRepository, IUploadService uploadService)
        {
            _customerRepository = customerRepository;
            _uploadService = uploadService;
        }

        // GET: Dashboard/Customers
        public async Task<IActionResult> Index(bool showDisabled = false)
        {
            ViewBag.ShowDisabled = showDisabled;
            var customers = await _customerRepository.GetAll();
            return View(customers.Where(c => !c.Deleted && (c.Enabled || showDisabled)));
        }

        // GET: Dashboard/Customers/Deleted
        public async Task<IActionResult> Deleted()
        {
            var customers = await _customerRepository.GetAll();
            return View(customers.Where(c => c.Deleted).ToList());
        }

        // GET: Dashboard/Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.Get((int) id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Dashboard/Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Image,Link")] Customer customer)
        {
            if (customer.Image == null)
                ModelState.AddModelError(nameof(customer.Image), "The Image field is required.");
            else
            {
                if (!(new[] {".png", ".jpg", ".jpeg"}).Contains(Path.GetExtension(customer.Image.FileName)?.ToLower()))
                    ModelState.AddModelError(nameof(customer.Image), "Invalid image type, only png and jpg images are allowed");

                if (customer.Image.Length < 1)
                    ModelState.AddModelError(nameof(customer.Image), "Filesize too small");
            }

            if (!ModelState.IsValid) return View(customer);

            customer.LogoPath =
                await _uploadService.Upload(customer.Image, "/images/uploads/customers", customer.Name);

            await _customerRepository.Create(customer);
            return RedirectToAction(nameof(Index));
        }

        // GET: Dashboard/Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.Get((int) id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Dashboard/Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Image,LogoPath,Link")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (customer.Image != null)
            {
                if (!(new[] {".png", ".jpg", ".jpeg"}).Contains(Path.GetExtension(customer.Image.FileName)?.ToLower()))
                    ModelState.AddModelError(nameof(customer.Image), "Invalid image type, only png and jpg images are allowed");

                if (customer.Image.Length == 0)
                    ModelState.AddModelError(nameof(customer.Image), "Filesize too small");
            }

            if (!ModelState.IsValid) return View(customer);

            try
            {
                if (customer.Image != null)
                {
                    if (customer.LogoPath != null)
                        await _uploadService.Delete(customer.LogoPath);

                    customer.LogoPath =
                        await _uploadService.Upload(customer.Image, "/images/uploads/customers", customer.Name);
                }

                await _customerRepository.Update(customer);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CustomerExists(customer.Id))
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

        // GET: Dashboard/Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.Get((int) id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Dashboard/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _customerRepository.Get(id);
            if (customer == null)
            {
                return NotFound();
            }

            customer.Deleted = true;

            await _customerRepository.Update(customer);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Restore(int? id)
        {
            var customer = await _customerRepository.Get(id ?? 0);
            if (customer == null)
            {
                return NotFound();
            }

            customer.Deleted = false;

            await _customerRepository.Update(customer);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleEnable(int? id)
        {
            var customer = await _customerRepository.Get(id ?? 0);
            if (customer == null)
            {
                return NotFound();
            }

            customer.Enabled = !customer.Enabled;

            await _customerRepository.Update(customer);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("api/dashboard/[controller]/[action]")]
        public async Task<ObjectResult> Autocomplete(string term = "")
        {
            return new ObjectResult(new
            {
                results = (await _customerRepository.Search(term)).Select(t => new {id = t.Id, text = t.Name})
            });
        }

        private async Task<bool> CustomerExists(int id)
        {
            return await _customerRepository.Get(id) != null;
        }
    }
}
