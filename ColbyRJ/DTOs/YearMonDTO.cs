namespace ColbyRJ.DTOs
{
    public class YearMonDTO
    {
        public string YearMon { get; set; }
        public int YMCount { get; set; }
    }
    public class YearMonEditDTO
    {
        [Display(Name = "Year")]
        public string YearStr { get; set; } = string.Empty;

        [Display(Name = "Year")]
        [Range(1910, 2050, ErrorMessage = "Please enter valid Year")]
        public int YearInt { get; set; } = DateTime.Now.Year;

        [Display(Name = "Mon")]
        public string MonStr { get; set; } = string.Empty;

        public int Id { get; set; }
    }
}
