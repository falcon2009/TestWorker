namespace TestWorker.ContentFile
{
    using Amazon.S3;
    using Amazon.S3.Model;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using TestWorker.Configuration;

    public sealed class S3ContentFileProvider : IContentFileProvider
    {
        private readonly IAmazonS3 client;
        private readonly IS3Configuration config;

        public S3ContentFileProvider(IS3Configuration config)
        {
            this.client = new AmazonS3Client();
            this.config = config;
        }

        public async Task<IContentFile> GetAsync(string key)
        {
            using GetObjectResponse response = await this.client.GetObjectAsync(config.BucketName, key);
            return await CreateContentFile(response.Key, response.ResponseStream);
        }

        public async Task<IEnumerable<IContentFile>> GetListAsync(string prefix)
        {
            IList<string> keyList = await this.client.GetAllObjectKeysAsync(config.BucketName, prefix, new Dictionary<string, object>());
            IContentFile[] contentFileArray = await Task.WhenAll(keyList.Select(async key => await CreateContentFile(key)));

            return contentFileArray;
        }

        public void Dispose()
        {
            this.client?.Dispose();
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
