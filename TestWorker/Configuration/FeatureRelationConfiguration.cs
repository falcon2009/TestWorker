namespace TestWorker.Configuration
{
    public interface IFeatureRelationConfiguration : IRelationConfiguration
    {
        string FeatureFlag { get; }
    }

    public class FeatureRelationConfiguration : RelationConfiguration, IFeatureRelationConfiguration
    {
        public string FeatureFlag { get; set; }
    }
}
