namespace ColbyRJ.Repository.IRepository
{
    public interface ISnippetCommentRepository
    {
        public Task<string> Create(SnippetCommentDTO commentDTO);
        public Task<int> Delete(int commentId);
        public Task<IEnumerable<SnippetCommentDTO>> GetComments(int snippetId);
        public Task<SnippetCommentDTO> GetComment(int commentId);
    }
}
