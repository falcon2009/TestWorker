﻿using System.IO;
using System.Threading.Tasks;
using TestWorker.ContentFileBase;

namespace TestWorker.ContentFileService
{
    public static class ContentFileHelper
    {
        public static async Task<IContentFile> CreateContentFileAsync(string key, Stream stream = null)
        {
            string name = Path.GetFileName(key);
            string path = Path.GetDirectoryName(key)
                              ?.Replace('\\', '/');

            ContentFile contentFile = new ContentFile
            {
                ContentType = "application/octem-stream",
                Name = name,
                Path = path,
            };

            if (stream != null)
            {
                contentFile.Stream = new MemoryStream();
                await stream.CopyToAsync(contentFile.Stream);
            }

            return contentFile;
        }
    }
}
