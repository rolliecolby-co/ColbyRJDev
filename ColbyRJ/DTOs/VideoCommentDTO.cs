namespace ColbyRJ.DTOs
{
    public class VideoCommentDTO : BaseCommentDTO
    {

        public int VideoId { get; set; }
        public VideoDTO Video { get; set; }
    }
}
