using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface IBlockRepository
    {
        Task Create(Block block);
        Task Update(Block block);
        Task Delete(int id);
    }
}
