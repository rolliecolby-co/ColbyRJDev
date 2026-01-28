namespace ColbyRJ.Models
{
    public class GalleryDecade
    {
        public int Id { get; set; }
        public string Decade { get; set; } = string.Empty;

        public ICollection<GalleryPhoto>? Photos { get; set; }
    }
}
