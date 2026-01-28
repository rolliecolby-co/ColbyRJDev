namespace ColbyRJ.DTOs
{
    public class TripCommentDTO : BaseCommentDTO
    {
        public int TripId { get; set; }
        public TripDTO Trip { get; set; }
    }
}
