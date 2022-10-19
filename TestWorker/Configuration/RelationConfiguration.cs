namespace TestWorker.Configuration
{
    public interface IRelationConfiguration
    {
        string Name { get; }

        string Type { get; }
    }

    public class RelationConfiguration : IRelationConfiguration
    {
        public string Name { get; set; }

        public string Type { get; set; }
    }
}
