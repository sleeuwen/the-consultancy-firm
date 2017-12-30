using System.Collections.Generic;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> Get(int id);
        Task<List<Customer>> GetAll();
        Task Create(Customer customer);
        Task Update(Customer customer);
        Task Delete(int id);
    }
}
