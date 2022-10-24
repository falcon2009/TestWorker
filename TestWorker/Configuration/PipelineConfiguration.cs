namespace TestWorker.Configuration
{
    public interface IPipelineConfiguration
    {
        IFeatureRelationConfiguration Decrypt { get; }

        IFeatureRelationConfiguration Convert { get; }

        IFeatureRelationConfiguration Encrypt { get; }
    }

    public class PipelineConfiguration : IPipelineConfiguration
    {
        public FeatureRelationConfiguration Decrypt { get; set; }

        public FeatureRelationConfiguration Convert { get; set; }

        public FeatureRelationConfiguration Encrypt { get; set; }

        IFeatureRelationConfiguration IPipelineConfiguration.Decrypt => Decrypt;

        IFeatureRelationConfiguration IPipelineConfiguration.Convert => Convert;

        IFeatureRelationConfiguration IPipelineConfiguration.Encrypt => Encrypt;
    }
}
