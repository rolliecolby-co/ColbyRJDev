namespace ColbyRJ.DTOs
{
    public class WhoAmIDTO
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Remarks { get; set; } = string.Empty;

        [Display(Name = "Order #")]
        public int? OrderBy { get; set; }

        public string Owner { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;

        public string AudioFilename { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;
        public string AudioNote { get; set; } = string.Empty;

        public string WithAudio { get; set; } = string.Empty;

        public DateTime DateUpdated { get; set; }
    }
}
   