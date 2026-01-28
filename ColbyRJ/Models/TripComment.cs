namespace ColbyRJ.Models
{
    public class TripComment : BaseCommentEntity
    {

        public int TripId { get; set; }
        public Trip Trip { get; set; }
    }
}
