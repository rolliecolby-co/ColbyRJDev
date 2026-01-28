namespace ColbyRJ.DTOs
{
    public class GallerySectionDTO
    {
        public int Id { get; set; }
        [Required]
        public string Section { get; set; }

        [Required]
        public int OrderBy { get; set; }

        public ICollection<GalleryPhotoDTO>? Photos { get; set; }
    }
}
