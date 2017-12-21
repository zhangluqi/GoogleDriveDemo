namespace CloudObject
{
    public class FileInformation
    {
        public string FileName { get; set; }

        public string FileId { get; set; }

        public long FileSize { get; set; }

        public bool IsFolder { get; set; }

        public string ParentId { get; set; }

        public string FilePath { get; set; }
    }
}
