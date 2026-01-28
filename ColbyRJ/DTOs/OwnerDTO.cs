namespace ColbyRJ.DTOs
{
    public class OwnerDTO
    {
        public string Owner { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
    }
    public class OwnerEditDTO
    {
        [Required]
        public string Owner { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public int Id { get; set; }
    }
}
