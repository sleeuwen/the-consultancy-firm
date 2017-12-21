using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
	public class CustomerRepository : ICustomerRepository
	{
		private readonly ApplicationDbContext _context;

		public CustomerRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public Task<Customer> Get(int id)
		{
			return _context.Customers.FindAsync(id);
		}

		public Task<List<Customer>> GetAll()
		{
			return _context.Customers.ToListAsync();
		}

		public Task Create(Customer customer)
		{
			_context.Add(customer);
			return _context.SaveChangesAsync();
		}

		public Task Update(Customer customer)
		{
			_context.Customers.Update(customer);
			return _context.SaveChangesAsync();
		}

		public Task Delete(int id)
		{
			var customer = Get(id).Result;
			_context.Customers.Remove(customer);
			return _context.SaveChangesAsync();
		}
	}
}