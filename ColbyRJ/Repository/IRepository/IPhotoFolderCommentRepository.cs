namespace ColbyRJ.Repository.IRepository
{
    public interface IPhotoFolderCommentRepository
    {
        public Task<string> Create(PhotoFolderCommentDTO commentDTO);
        public Task<int> Delete(int commentId);
        public Task<IEnumerable<PhotoFolderCommentDTO>> GetComments(int photoFolderId);
        public Task<PhotoFolderCommentDTO> GetComment(int commentId);
    }
}
