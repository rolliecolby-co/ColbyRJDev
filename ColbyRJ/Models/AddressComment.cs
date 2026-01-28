namespace ColbyRJ.Models
{
    public class AddressComment : BaseCommentEntity
    {
        public int AddressHistoryId { get; set; }
        public AddressHistory Address { get; set; }
    }
}
