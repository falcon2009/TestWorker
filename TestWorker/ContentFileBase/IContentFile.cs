namespace TestWorker.ContentFile
{
    using System;
    using System.IO;

    public interface IContentFile : IDisposable
    {
        string Name { get; }

        string Path { get; }

        string ContentType { get; }

        Stream Stream { get; }
    }
}
