namespace ColbyRJ.Models
{
    public class StoryChapter
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;

        [Column(TypeName = "Date")]
        public DateTime? ChapterDate { get; set; }

        public string YearStr { get; set; } = string.Empty;
        public string MonStr { get; set; } = string.Empty;
        public string YearMon { get; set; } = string.Empty;
        public string YearMonth { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public bool Active { get; set; } = false;

        [Column(TypeName = "Date")]
        public DateTime DateUpdated { get; set; }

        public string AudioFilename { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;

        public int StoryId { get; set; }
        public Story Story { get; set; }
        public ICollection<StoryChapterPhoto>? Photos { get; set; }
        public ICollection<StoryChapterComment>? Comments { get; set; }
    }
}
