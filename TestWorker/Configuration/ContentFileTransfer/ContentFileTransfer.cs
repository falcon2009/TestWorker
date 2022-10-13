namespace TestWorker.Configuration.ContentFileTransfer
{
    public interface IContentFileTransfer
    {
        string FileType { get; }

        string Source { get; }

        string Destination { get; }

        IProviderConfiguration Provider { get; }

        IManagerConfiguration Manager { get; }

        bool ShouldRemoveAfterUploading { get; }
    }

    public class ContentFileTransfer : IContentFileTransfer, IKey<string>
    {
        public string FileType { get; set; } 

        public string Source { get; set; }

        public string Destination { get; set; }

        public ProviderConfiguration Provider { get; set; }

        public ManagerConfiguration Manager { get; set; }

        public bool ShouldRemoveAfterUploading { get; set; }

        public string Key => $"{FileType}_{Source}";

        IProviderConfiguration IContentFileTransfer.Provider => Provider;

        IManagerConfiguration IContentFileTransfer.Manager => Manager;
    }
}
