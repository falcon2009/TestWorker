
using System;
using TestWorker.Configuration;
using TestWorker.Parsing;

namespace TestWorker.Converter
{
    public interface IConvertFactory
    {
        IConverter GetConverter(IRelationConfiguration configuration);
    }

    public class ConvertFactory : IConvertFactory
    {
        public IConverter GetConverter(IRelationConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration?.Type) || configuration.Type == "None")
            {
                return null;
            }

            return configuration.Type switch
            {
                "Parse" => GetParser(configuration.Name),
                _ => throw new ArgumentException("Invalid converter type"),
            };
        }

        private static IConverter GetParser(string name)
        {
            return name switch
            {
                "MT103" => new MT103Parser(),
                "MT940" => new MT940Parser(),
                "BACS" => new BacsParser(),
                _ => throw new ArgumentException("Invalid converter type"),
            };
        }
    }
}
