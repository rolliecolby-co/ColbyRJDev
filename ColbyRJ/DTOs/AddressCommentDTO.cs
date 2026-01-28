namespace ColbyRJ.DTOs
{
    public class AddressCommentDTO : BaseCommentDTO
    {
        public int AddressHistoryId { get; set; }
        public AddressDTO Address { get; set; }
    }
}
