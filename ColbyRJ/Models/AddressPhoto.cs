namespace ColbyRJ.Models
{
    public class AddressPhoto : BasePhotoEntity
    {

        public int AddressHistoryId { get; set; }
        public AddressHistory Address { get; set; }
    }
}
