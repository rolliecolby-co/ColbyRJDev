namespace ColbyRJ.DTOs
{
    public class PhotoFolderCommentDTO : BaseCommentDTO
    {

        public int PhotoFolderId { get; set; }
        public PhotoFolderDTO PhotoFolder { get; set; }
    }
}
