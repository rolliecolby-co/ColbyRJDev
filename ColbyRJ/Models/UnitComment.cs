namespace ColbyRJ.Models
{
    public class UnitComment : BaseCommentEntity
    {
        public int UnitHistoryId { get; set; }
        public UnitHistory Unit { get; set; }
    }
}
