namespace ColbyRJ.Repository.IRepository
{
    public interface IPhotoAlbumRepository
    {
        public Task<string> Create(PhotoAlbumCreateDTO photoAlbumDTO);
        public Task<int> Delete(int photoAlbumId);
        public Task<List<PhotoAlbumDTO>> GetPhotoAlbums();
        public Task<List<PhotoAlbumDTO>> GetActivePhotoAlbums();
        public Task<PhotoAlbumDTO> GetPhotoAlbum(int photoAlbumId);
        public Task<PhotoAlbumDTO> GetPhotoAlbumByKey(string key);
        public Task<PhotoAlbumDTO> Update(PhotoAlbumDTO photoAlbumDTO);
        public Task<string> UpdateGroupBy(GroupByEditDTO groupByDTO);
        public Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO);
    }
}
