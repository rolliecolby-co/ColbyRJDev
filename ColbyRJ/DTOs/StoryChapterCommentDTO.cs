namespace ColbyRJ.DTOs
{
    public class StoryChapterCommentDTO : BaseCommentDTO
    {
        public string StoryChapterKey { get; set; } = string.Empty;

        public int StoryChapterId { get; set; }
        public StoryChapterDTO StoryChapter { get; set; }
    }
}
