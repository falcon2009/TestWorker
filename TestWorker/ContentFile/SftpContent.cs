using Renci.SshNet;
using System.IO;
using System.Text;
using TestWorker.Configuration;

namespace TestWorker
{
    public class SftpContent
    {
        public SftpContent(IFtpConfiguration config)
        {
            using Stream stream = new MemoryStream(Encoding.ASCII.GetBytes(config.PrivateKey));
            PrivateKeyFile keyFile = new(stream);
            AuthenticationMethod[] authMethods = new[]
            {
                new PrivateKeyAuthenticationMethod(config.UserName, new []{ keyFile })
            };

            Client = new(new ConnectionInfo(config.Host, config.Port, config.UserName, authMethods));
        }
        protected SftpClient Client { get; }

    }
}
