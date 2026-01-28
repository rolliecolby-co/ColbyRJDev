namespace ColbyRJ.Models
{
    public class PhotoFolder : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string PathFolder { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public bool ShowFilename { get; set; } = false;

        public ICollection<PhotoFolderComment>? Comments { get; set; }

    }
}
