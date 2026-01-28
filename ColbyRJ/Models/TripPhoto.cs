namespace ColbyRJ.Models
{
    public class TripPhoto
    {
        public int Id { get; set; }
        public int Sort { get; set; }
        public string Caption { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;

        [Column(TypeName = "Date")]
        public DateTime DateUpdated { get; set; }

        public int? TripId { get; set; }
        public Trip? Trip { get; set; }
        public int? TripGroupId { get; set; }
        public TripGroup? TripGroup { get; set; }
        public int? TripSectionId { get; set; }
        public TripSection? TripSection { get; set; }
        public int? TripSubSectionId { get; set; }
        public TripSubSection? TripSubSection { get; set; }
    }
}
