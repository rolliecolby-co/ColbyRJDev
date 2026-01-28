namespace ColbyRJ.Repository.IRepository
{
    public interface IJobCommentRepository
    {
        public Task<string> Create(JobCommentDTO commentDTO);
        public Task<int> Delete(int commentId);
        public Task<IEnumerable<JobCommentDTO>> GetComments(int jobId);
        public Task<JobCommentDTO> GetComment(int commentId);
    }
}
