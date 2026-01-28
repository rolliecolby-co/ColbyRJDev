namespace ColbyRJ.DTOs
{
    public class SnippetCommentDTO : BaseCommentDTO
    {

        public int SnippetId { get; set; }
        public SnippetDTO Snippet { get; set; }
    }
}
