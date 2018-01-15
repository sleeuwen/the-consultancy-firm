using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface IDownloadLogRepository
    {
        Task Log(DownloadLog downloadLog);
    }
}
