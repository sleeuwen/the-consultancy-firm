using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Common;
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
            return _context.Downloads
                .Include(d => d.DownloadTags).ThenInclude(dt => dt.Tag)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<Download> GetAll()
        {
            return _context.Downloads;
        }

        public async Task Create(Download download)
        {
            _context.Downloads.Add(download);
            await _context.SaveChangesAsync();

            _context.ItemTranslations.Add(new ItemTranslation()
            {
                ContentType = Enumeration.ContentItemType.Download,
                IdNl = download.Id
            });
            await  _context.SaveChangesAsync();
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

        public Task<Download> GetLatest()
        {
            return _context.Downloads.OrderByDescending(d => d.Date).Take(1).FirstOrDefaultAsync();
        }

        public async Task<int> CreateCopy(int id)
        {
            var download = await Get(id);
            var downloadCopy = new Download
            {
                Date = DateTime.UtcNow,
                Description = download.Description,
                DownloadTags = download.DownloadTags.Select(d => new DownloadTag{TagId = d.TagId}).ToList(),
                File = download.File,
                Language = "en",
                Title = download.Title,
                AmountOfDownloads = 0,
                LastModified = DateTime.UtcNow,
                LinkPath = download.LinkPath
            };
            await _context.Downloads.AddAsync(downloadCopy);
            await _context.SaveChangesAsync();
            var itemTranslation = await _context.ItemTranslations.FirstOrDefaultAsync(d => d.IdNl == id);
            itemTranslation.IdEn = downloadCopy.Id;
            await _context.SaveChangesAsync();
            return downloadCopy.Id;
        }
            
    }
}
