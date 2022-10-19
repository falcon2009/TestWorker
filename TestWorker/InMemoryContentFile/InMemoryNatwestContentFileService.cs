using System.IO;
using System.Text;
using System.Threading.Tasks;
using TestWorker.ContentFileBase;
using TestWorker.ContentFileService;

namespace TestWorker.InMemoryContentFile
{
    public class InMemoryNatwestContentFileService : InMemoryContentFileServiceBase
    {
        private readonly string[] keyArray = new string[] {"input/first/fist.txt", "second.txt" , "input/third/third.txt", "forth.txt" };
        private readonly string[] valueArray = new string[] {"first content", "second content", "third content", "forth content" };
        public override async Task SetupStorageAsync()
        {
            for (int i = 0; i < keyArray.Length; i++)
            {
                using Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(valueArray[i]));
                IContentFile item = await ContentFileHelper.CreateContentFileAsync(keyArray[i], stream);
                Storage.Add(keyArray[i], item);
            }
        }
    }
}
