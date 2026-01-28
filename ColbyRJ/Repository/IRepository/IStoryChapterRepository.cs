namespace ColbyRJ.Repository.IRepository
{
    public interface IStoryChapterRepository
    {
        public Task<string> Create(StoryChapterCreateDTO chapterDTO);
        public Task<int> Delete(int chapterId);
        public Task<List<StoryChapterDTO>> GetChapters();
        public Task<StoryChapterDTO> GetChapter(int chapterId);
        public Task<StoryChapterDTO> GetStoryChapterByKey(string key);
        public Task<StoryChapterDTO> Update(StoryChapterDTO chapterDTO);
        public Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO);
    }
}
