namespace ColbyRJ.Repository.IRepository
{
    public interface IStoryChapterPhotoRepository
    {
        public Task<string> Create(StoryChapterPhotoDTO photoDTO);
        public Task<int> Delete(int photoId);
        public Task<int> DeletePhotoByPhotoUrl(string photoUrl);
        public Task<List<StoryChapterPhotoDTO>> GetPhotos(int chapterId);
        public Task<StoryChapterPhotoDTO> GetPhoto(int photoId);
        public Task<string> Update(StoryChapterPhotoDTO photoDTO);
    }
}
