using System.IO;

namespace TestWorker.MockService
{
    public interface IPgpService
    {
        byte[] Encrypt(byte[] inputData, byte[] publicKeyData, bool withIntergrityCheck = true, bool armor = true);
        byte[] Encrypt(Stream inputStream, byte[] publicKeyData, bool withIntergrityCheck = true, bool armor = true);
        byte[] Decrypt(byte[] inputData, byte[] publicKeyData, string password);
        byte[] Decrypt(Stream inputStream, byte[] publicKeyData, string password);
    }
}
