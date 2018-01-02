using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace TheConsultancyFirm.Services
{
    public class UploadService : IUploadService
    {
        private readonly IHostingEnvironment _environment;

        public UploadService(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public Task Delete(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            var fileInfo = new FileInfo(_environment.WebRootPath + filePath);
            if (fileInfo.Exists)
                fileInfo.Delete();

            return Task.CompletedTask;
        }

        public async Task<string> Upload(IFormFile file, string directory, string filename = null, string extension = null,
            FileMode fileMode = FileMode.Create)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));

            if (extension == null)
                extension = Path.GetExtension(file.FileName);

            string filePath;
            if (filename != null)
            {
                filePath = "/" + ($"{directory.Trim('/')}/{filename.Trim('/', '.')}.{extension.Trim('.')}".Trim('/'));
            }
            else
            {
                do
                {
                    filePath = "/" + ($"{directory.Trim('/')}/{Path.GetRandomFileName()}.{extension.Trim('.')}".Trim('/'));
                } while (new FileInfo(_environment.WebRootPath + filePath).Exists);
            }

            using (var fileStream = new FileStream(_environment.WebRootPath + filePath, fileMode))
            {
                await file.CopyToAsync(fileStream);
            }

            return filePath;
        }
    }
}
