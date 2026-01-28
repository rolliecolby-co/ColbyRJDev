namespace ColbyRJ.Models
{
    public class GalleryPhoto
    {
        public int Id { get; set; }

        public string Caption { get; set; } = string.Empty;

        public int OrderBy { get; set; }

        public int PhotoYearInt { get; set; }
        public int PhotoMonthInt { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;

        [Column(TypeName = "Date")]
        public DateTime DateUpdated { get; set; }


        public int GallerySectionId { get; set; }
        public GallerySection Section { get; set; }
        public int GalleryDecadeId { get; set; }
        public GalleryDecade Decade{ get; set; }
    }
}
