namespace ColbyRJ.DTOs
{
    public class UnitCommentDTO : BaseCommentDTO
    {
        public int UnitHistoryId { get; set; }
        public UnitDTO Unit { get; set; }
    }
}
