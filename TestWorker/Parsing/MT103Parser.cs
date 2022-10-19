namespace TestWorker.Parsing
{
    using TestWorker.Converter;

    public class MT103Parser : IParser, IConverter
    {
        public byte[] Convert(byte[] sourseData) => Parse(sourseData);

        public byte[] Parse(byte[] sourseData)
        {
            return sourseData;
        }
    }
}
