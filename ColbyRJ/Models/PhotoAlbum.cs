namespace ColbyRJ.Models
{
    public class PhotoAlbum : BaseEntity
    {
        public string Remarks { get; set; } = string.Empty;

        public ICollection<PhotoAlbumPhoto>? Photos { get; set; }
        public ICollection<PhotoAlbumComment>? Comments { get; set; }
    }
}
