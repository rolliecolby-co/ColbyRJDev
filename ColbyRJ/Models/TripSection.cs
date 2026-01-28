namespace ColbyRJ.Models
{
    public class TripSection
    {
        public int Id { get; set; }
        public int Sort { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        [Column(TypeName = "Date")]
        public DateTime DateUpdated { get; set; }

        public int TripGroupId { get; set; }
        public TripGroup TripGroup { get; set; }
        public ICollection<TripSubSection>? TripSubSections { get; set; }
        public ICollection<TripPhoto>? TripPhotos { get; set; }
    }
}
