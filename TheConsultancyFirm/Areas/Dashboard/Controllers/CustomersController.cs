using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
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
        public async Task<IActionResult> Create([Bind("Id,Name,Image,Link")] Customer customer)
        {
            if (ModelState.IsValid)
            {
	            if (customer.Image.Length > 0)
	            {
		            customer.Image.FileName.Replace(" ", "");
					customer.LogoPath = "/images/CustomerLogos/" + customer.Image.FileName;
		            using (var fileStream = new FileStream(_environment.WebRootPath + customer.LogoPath, FileMode.Create))
		            {
			            await customer.Image.CopyToAsync(fileStream);
		            }
	            }
	            else
	            {
		            ModelState.AddModelError("Image", "Filesize to small");
	            }
				await _customerRepository.Create(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
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

            if (ModelState.IsValid)
            {
                try
                {

	                if (customer.Image.Length > 0)
	                {
		                FileInfo file = new FileInfo(_environment.WebRootPath + customer.LogoPath);
		                if (file.Exists)
		                {
			                file.Delete();
		                }
						customer.Image.FileName.Replace(" ", "");
		                customer.LogoPath = "/images/CustomerLogos/" + customer.Image.FileName;
		                using (var fileStream = new FileStream(_environment.WebRootPath + customer.LogoPath, FileMode.Create))
		                {
			                await customer.Image.CopyToAsync(fileStream);
		                }
	                }
					await _customerRepository.Update(customer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
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
            return View(customer);
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

        private bool CustomerExists(int id)
        {
            return _customerRepository.Get(id) != null;
        }
    }
}
