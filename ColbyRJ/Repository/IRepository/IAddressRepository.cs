namespace ColbyRJ.Repository.IRepository
{
    public interface IAddressRepository
    {
        public Task<List<AddressDTO>> GetAddresses();
        public Task<List<AddressDTO>> GetBrowseAddresses();
        public Task<int> Delete(int commentId);
        public Task<string> Create(AddressDTO addressDTO);
        public Task<AddressDTO> GetAddress(int addressId);
        public Task<AddressDTO> Update(AddressDTO addressDTO);
        public Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO);
    }
}
