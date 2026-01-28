namespace ColbyRJ.Models
{
    public class AddressHistory
    {
        public int Id { get; set; }
        public string Who { get; set; } = string.Empty;

        [Column(TypeName = "Date")]
        public DateTime StartDate { get; set; }

        public string YearStr { get; set; } = string.Empty;
        public string MonStr { get; set; } = string.Empty;
        public string YearMon { get; set; } = string.Empty;
        public string YearMonth { get; set; } = string.Empty;

        public string HomeAddress { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;

        [Column(TypeName = "Date")]
        public DateTime DateUpdated { get; set; }

        public ICollection<AddressPhoto>? Photos { get; set; }
        public ICollection<AddressComment>? Comments { get; set; }
    }
}
