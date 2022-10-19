namespace TestWorker.Cryptography
{
    using System.IO;
    using System.Threading.Tasks;

    public interface ICryptographyDecryptor
    {
        Task<byte[]> DecryptAsync(byte[] sourceData);

        Task<byte[]> DecryptAsync(Stream sourceStream);
    }
}
