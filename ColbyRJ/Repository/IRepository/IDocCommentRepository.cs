namespace ColbyRJ.Repository.IRepository
{
    public interface IDocCommentRepository
    {
        public Task<string> Create(DocCommentDTO commentDTO);
        public Task<int> Delete(int commentId);
        public Task<IEnumerable<DocCommentDTO>> GetComments(int docId);
        public Task<DocCommentDTO> GetComment(int commentId);
    }
}
