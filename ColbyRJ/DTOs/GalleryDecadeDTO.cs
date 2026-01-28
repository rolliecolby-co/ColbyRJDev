namespace ColbyRJ.DTOs
{
    public class GalleryDecadeDTO
    {
        public int Id { get; set; }
        public string Decade { get; set; }

        public ICollection<GalleryPhotoDTO>? Photos { get; set; }
    }
}
