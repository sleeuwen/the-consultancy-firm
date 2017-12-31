using System.Linq;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface INewsRepository
    {
        NewsItem Get(int id);
        IQueryable<NewsItem> GetAll();
    }
}
