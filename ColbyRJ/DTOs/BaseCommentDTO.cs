namespace ColbyRJ.DTOs
{
    public class BaseCommentDTO
    {
        public int Id { get; set; }

        [Required]
        [RemarksValidator(Remarks = "The Comment")]
        public string Comments { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;

        public DateTime CommentDate { get; set; }
    }
}
