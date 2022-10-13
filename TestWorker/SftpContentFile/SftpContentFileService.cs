using Renci.SshNet.Sftp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWorker.Configuration;
using TestWorker.ContentFileBase;
using TestWorker.ContentFileService;

namespace TestWorker.SftpContentFile
{
    public sealed class SftpContentFileService : SftpContent, IContentFileService
    {
        public SftpContentFileService(ISftpConfiguration config) : base(config)
        { }

        public async Task<IContentFile> GetAsync(string key)
        {
            Client.Connect();
            using SftpFileStream stream = Client.OpenRead(key);
            Client.Disconnect();

            return await ContentFileHelper.CreateContentFileAsync(stream.Name, stream);
        }

        public async Task<IEnumerable<IContentFile>> GetListAsync(string prefix)
        {
            Client.Connect();
            IEnumerable<SftpFile> fileArray = Client.ListDirectory(prefix)?.Where(file => !file.IsDirectory);
            IContentFile[] contentFileArray = await Task.WhenAll(fileArray.Select(async item => await ContentFileHelper.CreateContentFileAsync(item.FullName)));
            Client.Disconnect();

            return contentFileArray;
        }

        public Task UploadAsync(IContentFile contentFile)
        {
            return Task.Run(() => {
                Client.Connect();
                string path = string.IsNullOrEmpty(contentFile.Path) ? contentFile.Name : $"{contentFile.Path}/{contentFile.Name}";
                Client.UploadFile(contentFile.Stream, path);
                Client.Disconnect();
            });
        }

        public Task DeleteAsync(string key)
        {
            return Task.Run(() => {
                Client.Connect();
                Client.DeleteFile(key);
                Client.Disconnect();
            });
        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}
