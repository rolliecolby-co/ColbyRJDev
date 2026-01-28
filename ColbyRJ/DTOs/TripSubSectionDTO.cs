namespace ColbyRJ.DTOs
{
    public class TripSubSectionCreateDTO
    {
        public int TripSectionId { get; set; }
    }
    public class TripSubSectionDTO
    {
        public int Id { get; set; }
        public int Sort { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        [Column(TypeName = "Date")]
        public DateTime DateUpdated { get; set; }

        [Display(Name = "Photos")]
        public string PhotoCount { get; set; } = string.Empty;

        public int TripSectionId { get; set; }
        public TripSectionDTO TripSection { get; set; }
        public ICollection<TripPhotoDTO>? TripPhotos { get; set; }
        public List<string> PhotoUrls { get; set; }
    }
}
