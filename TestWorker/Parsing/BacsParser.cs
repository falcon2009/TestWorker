namespace TestWorker.Parsing
{
    using TestWorker.Converter;

    public class BacsParser : IParser, IConverter
    {
        public byte[] Convert(byte[] sourseData) => Parse(sourseData);

        public byte[] Parse(byte[] sourseData)
        {
            return sourseData;
        }
    }
}
