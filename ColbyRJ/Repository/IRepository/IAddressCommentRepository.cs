namespace ColbyRJ.Repository.IRepository
{
    public interface IAddressCommentRepository
    {
        public Task<string> Create(AddressCommentDTO commentDTO);
        public Task<int> Delete(int commentId);
        public Task<IEnumerable<AddressCommentDTO>> GetComments(int addressId);
        public Task<AddressCommentDTO> GetComment(int commentId);
    }
}
