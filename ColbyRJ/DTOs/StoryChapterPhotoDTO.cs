namespace ColbyRJ.DTOs
{
    public class StoryChapterPhotoDTO : BasePhotoDTO
    {

        public int StoryChapterId { get; set; }
        public StoryChapterDTO Chapter { get; set; }
    }
}
