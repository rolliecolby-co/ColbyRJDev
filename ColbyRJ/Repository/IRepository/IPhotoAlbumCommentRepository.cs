namespace ColbyRJ.Repository.IRepository
{
    public interface IPhotoAlbumCommentRepository
    {
        public Task<string> Create(PhotoAlbumCommentDTO commentDTO);
        public Task<int> Delete(int commentId);
        public Task<IEnumerable<PhotoAlbumCommentDTO>> GetComments(int photoAlbumId);
        public Task<PhotoAlbumCommentDTO> GetComment(int commentId);
    }
}
