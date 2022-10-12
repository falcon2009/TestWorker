using Renci.SshNet.Sftp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestWorker.Configuration;

namespace TestWorker.ContentFile
{
    public sealed class SftpContentFileProvider : SftpContent, IContentFileProvider
    {
        public SftpContentFileProvider(IFtpConfiguration config) : base(config)
        { }

        public async Task<IContentFile> GetAsync(string key)
        {
            Client.Connect();
            using SftpFileStream stream = Client.OpenRead(key);
            Client.Disconnect();

            return await CreateContentFile(stream.Name, stream);
        }

        public async Task<IEnumerable<IContentFile>> GetListAsync(string prefix)
        {
            Client.Connect();
            IEnumerable<SftpFile> fileArray = Client.ListDirectory(prefix);
            IContentFile[] contentFileArray = await Task.WhenAll(fileArray.Select(async item => await CreateContentFile(item.FullName)));
            Client.Disconnect();

            return contentFileArray;
        }

        public void Dispose()
        {
            Client?.Dispose();
        }

        private static async Task<IContentFile> CreateContentFile(string key, Stream stream = null)
        {
            var name = Path.GetFileName(key);
            var path = Path.GetDirectoryName(key)
                              ?.Replace('\\', '/');

            var contentFile = new ContentFile
            {
                ContentType = "application/octem-stream",
                Name = name,
                Path = path,
            };

            if (stream != null)
            {
                contentFile.Stream = new MemoryStream();
                await stream.CopyToAsync(contentFile.Stream);
            }

            return contentFile;
        }
    }
}
