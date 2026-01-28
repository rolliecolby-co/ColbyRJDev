namespace ColbyRJ.Models
{
    public class Trip : BaseEntity
    {
        public int YearMonthInt { get; set; }
        public string Note { get; set; } = string.Empty;
        public int YearInt { get; set; }
        public int MonthInt { get; set; }

        public ICollection<TripGroup>? TripGroups { get; set; }
        public ICollection<TripPhoto>? TripPhotos { get; set; }
        public ICollection<TripComment>? Comments { get; set; }
    }
}
