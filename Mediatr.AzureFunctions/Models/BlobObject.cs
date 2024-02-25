namespace IsolatedMediatr.Models
{
    public class BlobObject
    {
        public BlobObject(string filePath, string fileName, long? contentLength)
        {
            FilePath = filePath;
            FileName = fileName;
            Size = contentLength;
        }

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public long? Size { get; set; }
    }
}
