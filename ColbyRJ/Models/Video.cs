namespace ColbyRJ.Models
{
    public class Video : BaseEntity
    {
        public string Remarks { get; set; } = string.Empty;

        public string VideoFilename { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;

        public ICollection<VideoComment>? Comments { get; set; }

    }
}
