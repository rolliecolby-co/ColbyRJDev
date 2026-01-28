namespace ColbyRJ.Repository.IRepository
{
    public interface IGalleryRepository
    {
        public Task<string> CreateSection(GallerySectionDTO sectionDTO);
        public Task<List<GallerySectionDTO>> GetSections();
        public Task<int> DeleteSection(int sectionId);
        public Task<GallerySectionDTO> GetSection(int sectionId);
        public Task<string> UpdateSection(GallerySectionDTO sectionDTO);

        public Task<string> CreateDecade(GalleryDecadeDTO decadeDTO);
        public Task<List<GalleryDecadeDTO>> GetDecades();
        public Task<int> DeleteDecade(int decadeId);
        public Task<GalleryDecadeDTO> GetDecade(int decadeId);
        public Task<string> UpdateDecade(GalleryDecadeDTO decadeDTO);

        public Task<string> CreatePhoto(GalleryPhotoDTO photoDTO);
        public Task<List<GalleryPhotoDTO>> GetPhotos();
        public Task<List<GalleryPhotoDTO>> GetAllPhotos();
        public Task<int> DeletePhoto(int photoId);
        public Task<GalleryPhotoDTO> GetPhoto(int photoId);
        public Task<string> UpdatePhoto(GalleryPhotoDTO photoDTO);
    }
}
