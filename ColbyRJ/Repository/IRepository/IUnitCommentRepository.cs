namespace ColbyRJ.Repository.IRepository
{
    public interface IUnitCommentRepository
    {
        public Task<string> Create(UnitCommentDTO commentDTO);
        public Task<int> Delete(int ucommentId);
        public Task<IEnumerable<UnitCommentDTO>> GetComments(int unitId);
        public Task<UnitCommentDTO> GetComment(int commentId);
    }
}
