namespace TestWorker.ContentFileBase
{
    using System;
    using System.Threading.Tasks;

    public interface IContentFileManager : IDisposable
    {
        Task UploadAsync(IContentFile contentFile);

        Task DeleteAsync(string key);
    }
}