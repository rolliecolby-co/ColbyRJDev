namespace ColbyRJ.Repository.IRepository
{
    public interface ISnippetPhotoRepository
    {
        public Task<string> Create(SnippetPhotoDTO photoDTO);
        public Task<int> Delete(int photoId);
        public Task<int> DeletePhotoByPhotoUrl(string photoUrl);
        public Task<List<SnippetPhotoDTO>> GetPhotos(int snippetId);
        public Task<SnippetPhotoDTO> GetPhoto(int photoId);
        public Task<string> Update(SnippetPhotoDTO photoDTO);
    }
}
