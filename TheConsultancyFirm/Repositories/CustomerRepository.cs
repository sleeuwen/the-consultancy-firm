using System;
using System.Collections.Generic;
using System.Linq;
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

		public Customer Get(int id)
		{
			return _context.Customers.Find(id);
		}

		public List<Customer> GetAll()
		{
			return _context.Customers.ToList();
		}

		public void Create(Customer customer)
		{
			_context.Customers.Add(customer);
		}

		public void Edit(Customer customer)
		{
			_context.Customers.Update(customer);
		}

		public void Delete(int id)
		{
			var customer = _context.Customers.Find(id);
			_context.Customers.Remove(customer);
		}
	}
}