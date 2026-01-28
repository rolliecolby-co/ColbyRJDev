namespace ColbyRJ.Models
{
    public class JobComment : BaseCommentEntity
    {
        public int JobHistoryId { get; set; }
        public JobHistory Job { get; set; }
    }
}
