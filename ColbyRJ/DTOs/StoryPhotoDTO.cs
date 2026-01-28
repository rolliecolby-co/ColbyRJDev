namespace ColbyRJ.DTOs
{
    public class StoryPhotoDTO : BasePhotoDTO
    {
        public int StoryId { get; set; }
        public StoryDTO Story { get; set; }
    }
}
