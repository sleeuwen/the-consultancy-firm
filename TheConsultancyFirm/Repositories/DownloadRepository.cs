using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
	public class DownloadRepository : IDownloadRepository
    {
	    private ApplicationDbContext _context;

	    public DownloadRepository(ApplicationDbContext context)
	    {
		    _context = context;
	    }

		public Task<Download> Get(int id)
	    {
		    return _context.Downloads.FirstOrDefaultAsync(c => c.Id == id);
		}

		public IQueryable<Download> GetAll()
	    {
		    return _context.Downloads;
		}

	    public async Task Create(Download download)
	    {
		    _context.Downloads.Add(download);
		    await _context.SaveChangesAsync();
	    }

	    public async Task Update(Download download)
	    {
			_context.Downloads.Update(download);
		    await _context.SaveChangesAsync();
		}

	    public async Task Delete(int id)
	    {
			var download = await Get(id);
		    _context.Downloads.Remove(download);
		    await _context.SaveChangesAsync();
		}
    }
}