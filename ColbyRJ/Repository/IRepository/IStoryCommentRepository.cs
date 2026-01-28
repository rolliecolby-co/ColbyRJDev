namespace ColbyRJ.Repository.IRepository
{
    public interface IStoryCommentRepository
    {
        public Task<string> Create(StoryCommentDTO commentDTO);
        public Task<int> Delete(int commentId);
        public Task<IEnumerable<StoryCommentDTO>> GetComments(int storyId);
        public Task<StoryCommentDTO> GetComment(int commentId);
    }
}
