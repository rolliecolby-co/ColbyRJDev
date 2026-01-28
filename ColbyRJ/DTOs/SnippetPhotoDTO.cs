namespace ColbyRJ.DTOs
{
    public class SnippetPhotoDTO : BasePhotoDTO
    {
        public int SnippetId { get; set; }
        public SnippetDTO Snippet { get; set; }
    }
}
