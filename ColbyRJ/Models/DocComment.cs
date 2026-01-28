namespace ColbyRJ.Models
{
    public class DocComment : BaseCommentEntity
    {
        public int DocId { get; set; }
        public Doc Doc { get; set; }
    }
}
