namespace Tandem.Context.Payments.Core.Infrastructure.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using Tandem.Context.Payments.Core.Infrastructure.Resources;

    public static class ContentTypeHelper
    {
        private const int SkipLinesInExtensionFile = 2;
        private static readonly ConcurrentDictionary<string, ContentType> ContentTypeLookup =
            new ConcurrentDictionary<string, ContentType>(
                Resources.FileExtensionLookup
                         .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                         .Skip(SkipLinesInExtensionFile)
                         .Where(item => item.Contains(':'))
                         .Select(item => item.Split(':').Select(entry => entry.Trim(' ', '"')).ToArray())
                         .Select(item => KeyValuePair.Create(item[0].ToLower(), new ContentType(item[1]))));

        public static ContentType GetContentTypeByExtension(string extension)
        {
            return ContentTypeLookup.GetValueOrDefault(extension.ToLower().TrimStart('.')) ?? ContentTypeLookup.GetValueOrDefault("*");
        }
    }
}
