using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface IDownloadLogRepository
    {
        Task Log(DownloadLog downloadLog);
        Task<Dictionary<DateTime, int>> GetDownloadsLastWeek(int id = 0);
    }
}
