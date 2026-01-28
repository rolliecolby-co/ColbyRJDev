namespace ColbyRJ.Repository.IRepository
{
    public interface IStoryRepository
    {
        public Task<string> Create(StoryCreateDTO storyDTO);
        public Task<int> Delete(int storyId);
        public Task<List<StoryDTO>> GetStories();
        public Task<List<StoryDTO>> GetActiveStories();
        public Task<StoryDTO> GetStory(int storyId);
        public Task<StoryDTO> GetStoryByKey(string key);
        public Task<StoryDTO> Update(StoryDTO storyDTO);
        public Task<string> UpdateGroupBy(GroupByEditDTO groupByDTO);
        public Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO);
    }
}
