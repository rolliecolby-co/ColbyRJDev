namespace ColbyRJ.Repository.IRepository
{
    public interface IVideoCommentRepository
    {
        public Task<string> Create(VideoCommentDTO commentDTO);
        public Task<int> Delete(int commentId);
        public Task<IEnumerable<VideoCommentDTO>> GetComments(int videoId);
        public Task<VideoCommentDTO> GetComment(int commentId);
    }
}
