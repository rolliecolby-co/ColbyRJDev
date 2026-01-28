namespace ColbyRJ.DTOs
{
    public class DocCommentDTO : BaseCommentDTO
    {
        public int DocId { get; set; }
        public DocDTO Doc { get; set; }
    }
}
