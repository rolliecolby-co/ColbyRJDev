namespace ColbyRJ.DTOs
{
    public class StoryCommentDTO : BaseCommentDTO
    {
        public int StoryId { get; set; }
        public StoryDTO Story { get; set; }
    }
}
