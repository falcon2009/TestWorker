namespace TestWorker.Converter
{
    public interface IParser
    {
        byte[] Parse(byte[] sourseData);
    }
}
