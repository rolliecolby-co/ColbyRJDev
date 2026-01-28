namespace ColbyRJ.Models
{
    public class TripGroup
    {
        public int Id { get; set; }
        public int Sort { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        [Column(TypeName = "Date")]
        public DateTime DateUpdated { get; set; }

        public int TripId { get; set; }
        public Trip Trip { get; set; }
        public ICollection<TripSection>? TripSections { get; set; }
        public ICollection<TripPhoto>? TripPhotos { get; set; }
    }
}
