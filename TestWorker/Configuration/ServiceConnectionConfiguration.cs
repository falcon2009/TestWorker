namespace TestWorker.Configuration
{
    public interface IServiceConnectionConfiguration : IRelationConfiguration
    {
        string Folder { get; }
    }

    public class ServiceConnectionConfiguration: RelationConfiguration, IServiceConnectionConfiguration
    {
        public string Folder { get; set; }
    }
}
