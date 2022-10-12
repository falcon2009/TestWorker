namespace TestWorker.ContentFile
{
    public interface IContentFileManagerFactory
    {
        IContentFileManager GetContentFileManager(string fileType, string master);
    }
}
