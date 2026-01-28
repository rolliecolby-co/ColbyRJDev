namespace ColbyRJ.Repository.IRepository
{
    public interface IStoryPhotoRepository
    {
        public Task<string> Create(StoryPhotoDTO photoDTO);
        public Task<int> Delete(int photoId);
        public Task<int> DeletePhotoByPhotoUrl(string photoUrl);
        public Task<List<StoryPhotoDTO>> GetPhotos(int storyId);
        public Task<StoryPhotoDTO> GetPhoto(int photoId);
        public Task<string> Update(StoryPhotoDTO photoDTO);

    }
}
