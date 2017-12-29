using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class CustomersController : Controller
    {
        private readonly IHostingEnvironment _environment;
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository, IHostingEnvironment environment)
        {
            _environment = environment;
            _customerRepository = customerRepository;

        }

        // GET: Dashboard/Customers
        public async Task<IActionResult> Index()
        {
            return View(await _customerRepository.GetAll());
        }

        // GET: Dashboard/Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.Get((int)id);
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
            if (!ModelState.IsValid) return View(customer);

            if (customer.Image?.Length > 0)
            {
                var extension = Path.GetExtension(customer.Image.FileName);
                if (extension != ".jpg" && extension != ".png" && extension != ".jpeg")
                {
                    ModelState.AddModelError("Image", "The uploaded file was not an image");
                    return View(customer);
                }
                customer.LogoPath = "/images/CustomerLogos/" + customer.Name.Replace(" ","") + extension;
                using (var fileStream = new FileStream(_environment.WebRootPath + customer.LogoPath, FileMode.Create))
                {
                    await customer.Image.CopyToAsync(fileStream);
                }
            }
            else
            {
                ModelState.AddModelError("Image", "Filesize to small");
                return View(customer);
            }
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

            if (!ModelState.IsValid) return View(customer);
            try
            {

                if (customer.Image.Length > 0)
                {
                    var file = new FileInfo(_environment.WebRootPath + customer.LogoPath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                    var extension = Path.GetExtension(customer.Image.FileName);
                    if (extension != ".jpg" && extension != ".png" && extension != ".jpeg")
                    {
                        ModelState.AddModelError("Image", "The uploaded file was not an image");
                        return View(customer);
                    }
                    customer.LogoPath = "/images/CustomerLogos/" + customer.Name.Replace(" ", "") + extension;
                    using (var fileStream = new FileStream(_environment.WebRootPath + customer.LogoPath, FileMode.Create))
                    {
                        await customer.Image.CopyToAsync(fileStream);
                    }
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

            var customer = await _customerRepository.Get((int)id);
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
            FileInfo file = new FileInfo(_environment.WebRootPath + customer.LogoPath);
            if (file.Exists)
            {
                file.Delete();
            }
            await _customerRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CustomerExists(int id)
        {
            return await _customerRepository.Get(id) != null;
        }
    }
}
