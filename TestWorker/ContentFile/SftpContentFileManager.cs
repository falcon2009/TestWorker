using System.Threading.Tasks;
using TestWorker.Configuration;

namespace TestWorker.ContentFile
{
    public sealed class SftpContentFileManager : SftpContent, IContentFileManager
    {
        public SftpContentFileManager(IFtpConfiguration config) : base(config)
        {}

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
