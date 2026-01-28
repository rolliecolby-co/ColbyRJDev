namespace ColbyRJ.Repository.IRepository
{
    public interface IPhotoAlbumPhotoRepository
    {
        public Task<string> Create(PhotoAlbumPhotoDTO photoDTO);
        public Task<int> Delete(int photoId);
        public Task<int> DeletePhotoByPhotoUrl(string photoUrl);
        public Task<List<PhotoAlbumPhotoDTO>> GetPhotos(int photoAlbumId);
        public Task<PhotoAlbumPhotoDTO> GetPhoto(int photoId);
        public Task<string> Update(PhotoAlbumPhotoDTO photoDTO);
    }
}
