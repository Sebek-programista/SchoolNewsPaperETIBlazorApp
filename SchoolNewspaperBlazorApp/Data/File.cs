namespace SchoolNewspaperBlazorApp.Data
{
    public class File
    {
        public int Id { get; set; }
        public required string FileType { get; set; }
        public required string FileName { get; set; }
        public required string FilePath { get; set; }
        public int ArticleID { get; set; }
    }
}
