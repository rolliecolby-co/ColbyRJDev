namespace ColbyRJ.Repository.IRepository
{
    public interface IAddressPhotoRepository
    {
        public Task<string> Create(AddressPhotoDTO photoDTO);
        public Task<int> Delete(int photoId);
        public Task<int> DeletePhotoByPhotoUrl(string photoUrl);
        public Task<List<AddressPhotoDTO>> GetPhotos(int addressId);
        public Task<AddressPhotoDTO> GetPhoto(int photoId);
        public Task<string> Update(AddressPhotoDTO photoDTO);
    }
}
