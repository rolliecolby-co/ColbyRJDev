namespace ColbyRJ.Repository.IRepository
{
    public interface ITopicRepository
    {
        public Task<string> Create(TopicAddDTO topicDTO);
        public Task<int> Delete(int topicDTO);
        public Task<List<TopicDTO>> GetTopics();
        public Task<TopicDTO> GetTopic(int topicId);
        public Task<string> Update(TopicDTO topicDTO);
        public Task<List<string>> GetSectionTopicsStr(string section);
    }
}
