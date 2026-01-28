namespace ColbyRJ.Models
{
    public class Story : BaseEntity
    {
        public string Body { get; set; } = string.Empty;

        public string AudioFilename { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;

        public ICollection<StoryChapter>? Chapters { get; set; }
        public ICollection<StoryPhoto>? Photos { get; set; }
        public ICollection<StoryComment>? Comments { get; set; }
    }
}
