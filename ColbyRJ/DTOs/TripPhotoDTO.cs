namespace ColbyRJ.DTOs
{
    public class TripPhotoDTO
    {
        public int Id { get; set; }
        public int Sort { get; set; }
        public string Caption { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;

        [Column(TypeName = "Date")]
        public DateTime DateUpdated { get; set; }

        public int? TripId { get; set; }
        public TripDTO? Trip { get; set; }
        public int? TripGroupId { get; set; }
        public TripGroupDTO? TripGroup { get; set; }
        public int? TripSectionId { get; set; }
        public TripSectionDTO? TripSection { get; set; }
        public int? TripSubSectionId { get; set; }
        public TripSubSectionDTO? TripSubSection { get; set; }
    }
}
