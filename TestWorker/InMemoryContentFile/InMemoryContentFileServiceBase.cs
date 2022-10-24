using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWorker.ContentFileBase;

namespace TestWorker.InMemoryContentFile
{
    public abstract class InMemoryContentFileServiceBase : IContentFileService
    {
        protected IDictionary<string, IContentFile> Storage { get;}
        private bool isDisposed = false;

        protected InMemoryContentFileServiceBase()
        {
            Storage = new Dictionary<string, IContentFile>();
        }

        public Task<IContentFile> GetAsync(string key)
        {
            // return Task.FromResult(Storage.First(item => item.Key == key).Value);
            throw new NotImplementedException("GetAsync InMemoryContentFileServiceBase is not implemented");
        }

        public Task<IEnumerable<IContentFile>> GetListAsync(string prefix)
        {
            Func<IContentFile, bool> predicate = string.IsNullOrEmpty(prefix) ? (item) => string.IsNullOrEmpty(item.Path)
                                                                              : (item) => item.Path == prefix;

            return Task.FromResult(Storage.Values.Where(predicate));
        }

        public Task UploadAsync(IContentFile contentFile)
        {
            string key = string.IsNullOrEmpty(contentFile.Path) ? contentFile.Name
                                                                : $"{contentFile.Path}/{contentFile.Name}";

            Storage.TryAdd(key, contentFile);

            return Task.CompletedTask;
        }

        public Task DeleteAsync(string key)
        {
            if (Storage.ContainsKey(key))
            {
                Storage.Remove(key);
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    string[] keyArray = Storage.Keys.ToArray();
                    foreach (string key in keyArray)
                    {
                        Storage.TryGetValue(key, out IContentFile item);
                        if (item != null)
                        {
                            item.Dispose();
                            Storage.Remove(key);
                        }
                    }
                }

                isDisposed = true;
            }
        }

        public abstract Task SetupStorageAsync();
    }
}
