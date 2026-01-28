namespace ColbyRJ.DTOs
{
    public class AddressPhotoDTO : BasePhotoDTO
    {

        public int AddressHistoryId { get; set; }
        public AddressDTO Address { get; set; }
    }
}
