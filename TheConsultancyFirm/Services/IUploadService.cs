using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TheConsultancyFirm.Services
{
    public interface IUploadService
    {
        Task Delete(string filePath);

        Task<string> Upload(IFormFile file, string directory, string filename = null, string extension = null,
            FileMode fileMode = FileMode.Create);
    }
}
