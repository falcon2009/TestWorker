namespace TestWorker.Cryptography
{
    using System.IO;
    using System.Threading.Tasks;

    public interface ICryptographyEncryptor
    {
        Task<byte[]> EncryptAsync(byte[] sourceData);

        Task<byte[]> EncryptAsync(Stream sourceStream);
    }
}
