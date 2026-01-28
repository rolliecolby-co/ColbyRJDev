namespace ColbyRJ.DTOs
{
    public class JobCommentDTO : BaseCommentDTO
    {
        public int JobHistoryId { get; set; }
        public JobDTO Job { get; set; }
    }
}
