using System.Threading.Tasks;

namespace TestWorker.InMemoryContentFile
{
    public class InMemoryAccessPayContentFileService : InMemoryContentFileServiceBase
    {
        public override Task SetupStorageAsync()
        {
            return Task.CompletedTask;
        }
    }
}
