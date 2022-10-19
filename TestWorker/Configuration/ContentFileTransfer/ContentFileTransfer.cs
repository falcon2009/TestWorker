namespace TestWorker.Configuration.ContentFileTransfer
{
    public interface IContentFileTransfer
    {
        string FileType { get; }

        string Source { get; }

        string Destination { get; }

        IProviderConfiguration Provider { get; }

        IServiceConnectionConfiguration Manager { get; }

        IPipelineConfiguration Pipeline { get; }

        IRetryPolicyConfiguration RetryPolicy { get; }

        bool ShouldRemoveAfterUploading { get; }
    }

    public class ContentFileTransfer : IContentFileTransfer, IKey<string>
    {
        public string FileType { get; set; } 

        public string Source { get; set; }

        public string Destination { get; set; }

        public ProviderConfiguration Provider { get; set; }

        public ServiceConnectionConfiguration Manager { get; set; }

        public PipelineConfiguration Pipeline { get; set; }

        public RetryPolicyConfiguration RetryPolicy { get; set;  }

        public bool ShouldRemoveAfterUploading { get; set; }

        public string Key => $"{FileType}_{Source}_{Destination}";

        IRetryPolicyConfiguration IContentFileTransfer.RetryPolicy => RetryPolicy;

        IProviderConfiguration IContentFileTransfer.Provider => Provider;

        IServiceConnectionConfiguration IContentFileTransfer.Manager => Manager;

        IPipelineConfiguration IContentFileTransfer.Pipeline => Pipeline;
    }
}
