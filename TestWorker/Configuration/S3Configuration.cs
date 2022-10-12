namespace TestWorker.Configuration
{
    public interface IS3Configuration
    {
        public string BucketName { get; }
    }

    public class S3Configuration : IS3Configuration
    {
        public string BucketName { get; set; }
    }
}
