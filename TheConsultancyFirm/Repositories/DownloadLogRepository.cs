using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

            var download = await _context.Downloads.FindAsync(downloadLog.DownloadId);
            download.AmountOfDownloads += 1;
            _context.Downloads.Update(download);

            await _context.SaveChangesAsync();
        }

        public async Task<Dictionary<DateTime, int>> GetDownloadsLastWeek(int? id)
        {
            var lastWeek = DateTime.UtcNow.AddDays(-7);

            var dictionary = await _context.DownloadLogs.Where(d => d.Date >= lastWeek && (id == null || d.DownloadId == id))
                .GroupBy(d => d.Date.Date)
                .Select(d => new
                {
                    Value = d.Count(),
                    Day = d.Key.Date
                }).ToDictionaryAsync(d => d.Day, d => d.Value);

            return dictionary;
        }
    }
}
