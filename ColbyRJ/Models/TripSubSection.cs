namespace ColbyRJ.Models
{
    public class TripSubSection
    {
        public int Id { get; set; }
        public int Sort { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        [Column(TypeName = "Date")]
        public DateTime DateUpdated { get; set; }

        public int TripSectionId { get; set; }
        public TripSection TripSection { get; set; }
        public ICollection<TripPhoto>? TripPhotos { get; set; }
    }
}
