namespace TestWorker.ContentFileBase
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IContentFileProvider : IDisposable
    {
        Task<IContentFile> GetAsync(string key);

        Task<IEnumerable<IContentFile>> GetListAsync(string prefix);
    }
}
