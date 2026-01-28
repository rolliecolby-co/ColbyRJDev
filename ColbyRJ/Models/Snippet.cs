namespace ColbyRJ.Models
{
    public class Snippet : BaseEntity
    {
        public string Body { get; set; } = string.Empty;

        public string AudioFilename { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;

        public ICollection<SnippetPhoto>? Photos { get; set; }
        public ICollection<SnippetComment>? Comments { get; set; }
    }
}
