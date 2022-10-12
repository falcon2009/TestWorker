namespace TestWorker.ContentFile
{
    using Amazon.S3;
    using Amazon.S3.Model;
    using System.Threading.Tasks;

    public sealed class S3ContentFileManager : IContentFileManager
    {
        private readonly IAmazonS3 client;
        private string Bucket { get; set; }

        public S3ContentFileManager(IAmazonS3 client)
        {
            this.client = client;
        }

        public void SetBucket(string bucket)
        {
            Bucket = bucket;
        }

        public async Task DeleteAsync(string key)
        {
            DeleteObjectRequest request = new DeleteObjectRequest
            {
                BucketName = Bucket,
                Key = key,
            };

            await this.client.DeleteObjectAsync(request);
        }

        public async Task UploadAsync(IContentFile contentFile)
        {
            string key = string.IsNullOrEmpty(contentFile.Path) ? contentFile.Name : $"{contentFile.Path}/{contentFile.Name}";
            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = Bucket,
                Key = key,
                InputStream = contentFile.Stream,
                ContentType = contentFile.ContentType,
            };

            await this.client.PutObjectAsync(request);
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
