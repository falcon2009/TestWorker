using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWorker.Configuration;
using TestWorker.ContentFileBase;
using TestWorker.ContentFileService;

namespace TestWorker.S3ContentFile
{
    public sealed class S3ContentFileService : IContentFileService
    {
        private readonly IAmazonS3 client;
        private readonly IS3Configuration config;

        public S3ContentFileService(IS3Configuration config)
        {
            this.client = new AmazonS3Client();
            this.config = config;
        }

        public async Task<IContentFile> GetAsync(string key)
        {
            using GetObjectResponse response = await this.client.GetObjectAsync(config.BucketName, key);

            return await ContentFileHelper.CreateContentFileAsync(response.Key, response.ResponseStream);
        }

        public async Task<IEnumerable<IContentFile>> GetListAsync(string prefix)
        {
            IList<string> keyList = await this.client.GetAllObjectKeysAsync(config.BucketName, prefix, new Dictionary<string, object>());
            IContentFile[] contentFileArray = await Task.WhenAll(keyList.Select(async key => await ContentFileHelper.CreateContentFileAsync(key)));

            return contentFileArray;
        }

        public async Task UploadAsync(IContentFile contentFile)
        {
            string key = string.IsNullOrEmpty(contentFile.Path) ? contentFile.Name : $"{contentFile.Path}/{contentFile.Name}";
            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = config.BucketName,
                Key = key,
                InputStream = contentFile.Stream,
                ContentType = contentFile.ContentType,
            };

            await this.client.PutObjectAsync(request);
        }

        public async Task DeleteAsync(string key)
        {
            DeleteObjectRequest request = new DeleteObjectRequest
            {
                BucketName = config.BucketName,
                Key = key,
            };

            await this.client.DeleteObjectAsync(request);
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
