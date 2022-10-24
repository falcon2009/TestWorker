using System.Threading.Tasks;
using TestWorker.Model;

namespace TestWorker.MockService
{
    public interface IFeatureFlagService
    {
        Task<bool> IsEnabledAsync<T>(int? cacheTimeSeconds = null) where T : class;
    }

    public class FeatureFlagService : IFeatureFlagService
    {
        public Task<bool> IsEnabledAsync<T>(int? cacheTimeSeconds = null) where T : class
        {
            return Task.FromResult(false);
        }
    }
}
