namespace ColbyRJ.Models
{
    public class GallerySection
    {
        public int Id { get; set; }
        public string Section { get; set; } = string.Empty;

        public int OrderBy { get; set; }

        public ICollection<GalleryPhoto>? Photos { get; set; }
    }
}
