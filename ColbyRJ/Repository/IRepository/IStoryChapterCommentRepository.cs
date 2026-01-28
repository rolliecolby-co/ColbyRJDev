namespace ColbyRJ.Repository.IRepository
{
    public interface IStoryChapterCommentRepository
    {
        public Task<string> Create(StoryChapterCommentDTO commentDTO);
        public Task<int> Delete(int commentId);
        public Task<IEnumerable<StoryChapterCommentDTO>> GetComments(int chapterId);
        public Task<StoryChapterCommentDTO> GetComment(int commentId);
    }
}
