using System.Collections.Generic;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
	public interface ICustomerRepository
	{
		Customer Get(int id);
		List<Customer> GetAll();
		void Create(Customer customer);
		void Edit(Customer customer);
		void Delete(int id);
	}
}