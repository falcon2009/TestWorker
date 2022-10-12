using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;

namespace TestWorker.Configuration
{
    public interface IConfigurationProvider<out TConfiguration> : IEnumerable<TConfiguration> where TConfiguration : class
    {
    }

    public class InMemoryConfigurationProviderGeneric<TConfiguration> : IConfigurationProvider<TConfiguration> where TConfiguration : class
    {
        private readonly ConcurrentDictionary<string, TConfiguration> storage = new();

        public void AddSettingsProvider(string name, TConfiguration provider)
        {
            if (!storage.ContainsKey(name))
            {
                storage.TryAdd(name, provider);
            }
        }

        public IEnumerator<TConfiguration> GetEnumerator()
        {
            return storage.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return storage.GetEnumerator();
        }
    }
}
