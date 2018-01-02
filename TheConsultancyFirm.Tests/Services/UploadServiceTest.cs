using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Moq;
using TheConsultancyFirm.Services;
using Xunit;

namespace TheConsultancyFirm.Tests.Services
{
    public class UploadServiceTest : IDisposable
    {
        private readonly string _tempDir;
        private readonly IHostingEnvironment _environment;

        public UploadServiceTest()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_tempDir);

            var environment = new Mock<IHostingEnvironment>();
            environment.Setup(env => env.WebRootPath).Returns(_tempDir);
            _environment = environment.Object;
        }

        public void Dispose()
        {
            Directory.Delete(_tempDir, true);
        }

        [Fact]
        public async void UploadCreatesFile()
        {
            var uploadService = new UploadService(_environment);
            var filename = "test.txt";
            var content = "Sample text: " + nameof(UploadCreatesFile);

            var filePath = await uploadService.Upload(FileMock(filename, content), "/", "test", "txt", FileMode.Create);
            Assert.Equal($"/{filename}", filePath);
            Assert.Equal(1, Directory.EnumerateFileSystemEntries(_tempDir).Count());
            Assert.Equal(filename, Path.GetFileName(Directory.EnumerateFileSystemEntries(_tempDir).First()));
            Assert.Equal(content, File.ReadAllText(Directory.EnumerateFileSystemEntries(_tempDir).First()));
        }

        [Fact]
        public async void UploadGeneratesFilename()
        {
            var uploadService = new UploadService(_environment);
            var filename = "test.md";
            var content = "Sample text: " + nameof(UploadGeneratesFilename);

            var filePath = await uploadService.Upload(FileMock(filename, content), "/", null, "txt", FileMode.Create);
            Assert.NotEqual($"/{filename}", filePath);
            Assert.Equal(1, Directory.EnumerateFileSystemEntries(_tempDir).Count());
            Assert.NotEqual(filename, Path.GetFileName(Directory.EnumerateFileSystemEntries(_tempDir).First()));
            Assert.Equal(content, File.ReadAllText(Directory.EnumerateFileSystemEntries(_tempDir).First()));
        }

        [Fact]
        public async void UploadUsesFileExtension()
        {
            var uploadService = new UploadService(_environment);
            var filename = "test.rtf";
            var content = "Sample text: " + nameof(UploadUsesFileExtension);

            var filePath = await uploadService.Upload(FileMock(filename, content), "/", "test", null, FileMode.Create);
            Assert.Equal($"/{filename}", filePath);
            Assert.Equal(1, Directory.EnumerateFileSystemEntries(_tempDir).Count());
            Assert.Equal(filename, Path.GetFileName(Directory.EnumerateFileSystemEntries(_tempDir).First()));
            Assert.Equal(content, File.ReadAllText(Directory.EnumerateFileSystemEntries(_tempDir).First()));
        }

        [Fact]
        public async void UploadGeneratesFilenameAndUsesFileExtension()
        {
            var uploadService = new UploadService(_environment);
            var filename = "test.html";
            var content = "Sample text: " + nameof(UploadGeneratesFilenameAndUsesFileExtension);

            var filePath = await uploadService.Upload(FileMock(filename, content), "/", null, null, FileMode.Create);
            Assert.NotEqual($"/{filename}", filePath);
            Assert.Equal(".html", Path.GetExtension(filePath));
            Assert.Equal(1, Directory.EnumerateFileSystemEntries(_tempDir).Count());
            Assert.NotEqual(filename, Path.GetFileName(Directory.EnumerateFileSystemEntries(_tempDir).First()));
            Assert.Equal(".html", Path.GetExtension(Directory.EnumerateFileSystemEntries(_tempDir).First()));
            Assert.Equal(content, File.ReadAllText(Directory.EnumerateFileSystemEntries(_tempDir).First()));
        }

        [Fact]
        public async void UploadThrowsException()
        {
            var uploadService = new UploadService(null);

            ArgumentException argEx;
            argEx = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                uploadService.Upload(null, "/", null, null, FileMode.Create));
            Assert.Equal("file", argEx.ParamName);

            argEx = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                uploadService.Upload(FileMock("", ""), null, null, null, FileMode.Create));
            Assert.Equal("directory", argEx.ParamName);
        }

        private static IFormFile FileMock(string filename, string content)
        {
            var stream = new MemoryStream(new UTF8Encoding().GetBytes(content));

            return new FormFile(stream, 0, stream.Length, "file", filename);
        }
    }
}
