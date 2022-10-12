namespace TestWorker.ContentFile
{
    using System.IO;

    public sealed class ContentFile : IContentFile
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string ContentType { get; set; }

        public Stream Stream { get; set; }

        public void Dispose()
        {
            this.Stream?.Dispose();
        }
    }
}
