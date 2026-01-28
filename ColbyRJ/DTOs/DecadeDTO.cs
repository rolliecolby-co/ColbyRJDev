namespace ColbyRJ.DTOs
{
    public class DecadeDTO
    {
        [Display(Name = "Decade")]
        public string Decade { get; set; } = string.Empty;
        public int DecadeCount { get; set; }
    }
}
