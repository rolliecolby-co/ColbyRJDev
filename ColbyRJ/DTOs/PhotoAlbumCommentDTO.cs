namespace ColbyRJ.DTOs
{
    public class PhotoAlbumCommentDTO : BaseCommentDTO
    {

        public int PhotoAlbumId { get; set; }
        public PhotoAlbumDTO PhotoAlbum { get; set; }
    }
}
