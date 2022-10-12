using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TestWorker
{
    public class InMemoryTreasuryExportProvider : IConfigurationProvider<ITreasuryConfigurationExport>
    {
        private readonly ConcurrentDictionary<string, ITreasuryConfigurationExport> storage;

        public InMemoryTreasuryExportProvider()
        {
            storage = new ConcurrentDictionary<string, ITreasuryConfigurationExport>();
        }

        public void AddSettingsProvider(string name, ITreasuryConfigurationExport provider)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<ITreasuryConfigurationExport> GetEnumerator()
        {
            return storage.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return storage.GetEnumerator();
        }
    }
}
