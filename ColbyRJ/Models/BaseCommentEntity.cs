namespace ColbyRJ.Models
{
    public class BaseCommentEntity
    {
        public int Id { get; set; }

        public string Comments { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;

        public DateTime CommentDate { get; set; }
    }
}
