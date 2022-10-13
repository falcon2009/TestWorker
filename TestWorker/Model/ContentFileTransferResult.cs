namespace TestWorker.Model
{
    public interface IContentFileTransferResult
    {
        string Source { get; }
        string Destination { get; }
        string FileType { get; }
        string FileName { get; }
    }

    public class ContentFileTransferResult : IContentFileTransferResult
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
    }
}
