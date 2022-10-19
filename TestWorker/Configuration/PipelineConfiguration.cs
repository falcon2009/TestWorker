namespace TestWorker.Configuration
{
    public interface IPipelineConfiguration
    {
        IRelationConfiguration Decrypt { get; }

        IRelationConfiguration Convert { get; }

        IRelationConfiguration Encrypt { get; }
    }

    public class PipelineConfiguration : IPipelineConfiguration
    {
        public RelationConfiguration Decrypt { get; set; }

        public RelationConfiguration Convert { get; set; }

        public RelationConfiguration Encrypt { get; set; }

        IRelationConfiguration IPipelineConfiguration.Decrypt => Decrypt;

        IRelationConfiguration IPipelineConfiguration.Convert => Convert;

        IRelationConfiguration IPipelineConfiguration.Encrypt => Encrypt;
    }
}
