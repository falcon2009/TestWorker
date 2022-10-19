using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorker.Configuration;

namespace TestWorker.Cryptography
{
    public class PgpEncryptor : ICryptographyEncryptor
    {
        private readonly IPgpEncryption encryption;

        public PgpEncryptor(IPgpEncryption encryption)
        {
            this.encryption = encryption;
        }

        public Task<byte[]> EncryptAsync(byte[] sourceData)
        {
            if (encryption == null)
            {
                return Task.FromResult(sourceData);
            }

            using Stream sourceStream = new MemoryStream(sourceData);

            return EncryptAsync(sourceStream);
        }

        public async Task<byte[]> EncryptAsync(Stream sourceStream)
        {
            if (encryption == null)
            {
                return await GetByteFromStreamAsync(sourceStream);
            }

            using MemoryStream keyStream = new MemoryStream(Encoding.UTF8.GetBytes(encryption.PublicKey));

            return await EncryptAsync(sourceStream, keyStream);
        }

        private async Task<byte[]> GetByteFromStreamAsync(Stream sourceStream)
        {
            using MemoryStream buffer = new MemoryStream();
            await sourceStream.CopyToAsync(buffer);

            return buffer.ToArray();
        }

        private async Task<byte[]> EncryptAsync(Stream sourceStream, Stream keyStream)
        {
            byte[] dataByteArray = await GetByteFromStreamAsync(sourceStream);
            byte[] keyByteArray = await GetByteFromStreamAsync(keyStream);
            string dataString = Encoding.UTF8.GetString(dataByteArray);
            string keyByteString = Encoding.UTF8.GetString(keyByteArray);

            return Encoding.UTF8.GetBytes($"{dataString}{keyByteString}");

        }
    }
}
