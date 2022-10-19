using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorker.Configuration;

namespace TestWorker.Cryptography
{
    public class PgpDecryptor : ICryptographyDecryptor
    {
        private readonly IPgpDecryption decryption;

        public PgpDecryptor(IPgpDecryption decryption)
        {
            this.decryption = decryption;
        }

        public Task<byte[]> DecryptAsync(byte[] sourceData)
        {
            if (decryption == null)
            {
                return Task.FromResult(sourceData);
            }

            using Stream sourceStream = new MemoryStream(sourceData);

            return DecryptAsync(sourceStream);
        }

        public async Task<byte[]> DecryptAsync(Stream sourceStream)
        {
            if (decryption == null)
            {
                return await GetByteFromStreamAsync(sourceStream);
            }

            using MemoryStream keyStream = new MemoryStream(Encoding.UTF8.GetBytes(decryption.PrivateKey));

            return await DecryptAsync(sourceStream, keyStream);
        }

        private async Task<byte[]> GetByteFromStreamAsync(Stream sourceStream)
        {
            using MemoryStream buffer = new MemoryStream();
            await sourceStream.CopyToAsync(buffer);

            return buffer.ToArray();
        }

        private async Task<byte[]> DecryptAsync(Stream sourceStream, Stream keyStream)
        {
            byte[] dataByteArray = await GetByteFromStreamAsync(sourceStream);
            byte[] keyByteArray = await GetByteFromStreamAsync(keyStream);
            string dataString = Encoding.UTF8.GetString(dataByteArray);
            string  keyByteString = Encoding.UTF8.GetString(keyByteArray);
            string result = dataString.Replace(keyByteString, string.Empty);

            return Encoding.UTF8.GetBytes(result);
        }

    }
}
