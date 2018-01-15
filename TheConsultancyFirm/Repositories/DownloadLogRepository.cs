using System.Threading.Tasks;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class DownloadLogRepository : IDownloadLogRepository
    {
        private readonly ApplicationDbContext _context;

        public DownloadLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Log(DownloadLog downloadLog)
        {
            await _context.DownloadLogs.AddAsync(downloadLog);

            var download = _context.Downloads.Find(downloadLog.DownloadId);
            download.AmountOfDownloads += 1;
            _context.Downloads.Update(download);

            await _context.SaveChangesAsync();
        }
    }
}
