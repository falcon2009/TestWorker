namespace TestWorker.Configuration.Connection
{
    public interface IS3Connection
    {
        public string Name { get; }
        public IS3Configuration S3Configuration { get; }
    }

    public class S3Connection : IS3Connection, IKey<string>
    {
        public string Name { get; set; }
        public S3Configuration S3Configuration { get; set; }
        public string Key => Name;

        IS3Configuration IS3Connection.S3Configuration => S3Configuration;
    }
}
