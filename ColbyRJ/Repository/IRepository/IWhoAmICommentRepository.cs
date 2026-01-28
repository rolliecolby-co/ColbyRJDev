namespace ColbyRJ.Repository.IRepository
{
    public interface IWhoAmICommentRepository
    {
        public Task<string> Create(WhoAmICommentDTO commentDTO);
        public Task<int> Delete(int commentId);
        public Task<IEnumerable<WhoAmICommentDTO>> GetComments(string ownerEmail);
        public Task<IEnumerable<WhoAmICommentDTO>> GetOwnerComments(string owner);
        public Task<IEnumerable<WhoAmICommentDTO>> GetAllComments();
        public Task<WhoAmICommentDTO> GetComment(int commentId);
    }
}
