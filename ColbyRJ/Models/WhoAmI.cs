namespace ColbyRJ.Models
{
    public class WhoAmI
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public int? OrderBy { get; set; }

        public string Owner { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;

        public string AudioFilename { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;
        public string AudioNote { get; set; } = string.Empty;


        [Column(TypeName = "Date")]
        public DateTime DateUpdated { get; set; }
    }
}
