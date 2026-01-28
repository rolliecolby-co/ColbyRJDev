namespace ColbyRJ.Repository.IRepository
{
    public interface IBaseRepository
    {
        public Task<BrowseDTO> GetBrowsing(string byWhat);

        public Task<List<PhotoAlbumDTO>> GetPhotoAlbums();
        public Task<List<PhotoFolderDTO>> GetPhotoFolders();
        public Task<List<SnippetDTO>> GetSnippets();
        public Task<List<StoryDTO>> GetStories();
        public Task<List<VideoDTO>> GetVideos();

        public Task<List<PhotoAlbumDTO>> GetPhotoAlbums(string groupedBy);
        public Task<List<PhotoFolderDTO>> GetPhotoFolders(string groupedBy);
        public Task<List<SnippetDTO>> GetSnippets(string groupedBy);
        public Task<List<StoryDTO>> GetStories(string groupedBy);
        public Task<List<VideoDTO>> GetVideos(string groupedBy);

        public Task<List<PhotoAlbumDTO>> GetPhotoAlbumsByDecade(string decadeStr);
        public Task<List<PhotoFolderDTO>> GetPhotoFoldersByDecade(string decadeStr);
        public Task<List<SnippetDTO>> GetSnippetsByDecade(string decadeStr);
        public Task<List<StoryDTO>> GetStoriesByDecade(string decadeStr);
        public Task<List<VideoDTO>> GetVideosByDecade(string decadeStr);
        public Task<BrowseTally> GetBrowseTally();
        public Task<ElementTally> GetElementTally();

        public Task<List<ElementDTO>> GetElements();
    }
}
