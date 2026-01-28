namespace ColbyRJ.DTOs
{
    public class CommentDTO
    {
        public string Comments { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;

        public DateTime CommentDate { get; set; }
    }
}
