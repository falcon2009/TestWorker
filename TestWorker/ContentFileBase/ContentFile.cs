﻿namespace TestWorker.ContentFileBase
{
    using System;
    using System.IO;

    public interface IContentFile : IDisposable
    {
        string Name { get; }

        string Path { get; }

        string ContentType { get; }

        MemoryStream Stream { get; }
    }

    public sealed class ContentFile : IContentFile
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string ContentType { get; set; }

        public MemoryStream Stream { get; set; }

        public void Dispose()
        {
            this.Stream?.Dispose();
        }
    }
}
