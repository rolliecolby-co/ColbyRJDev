namespace ColbyRJ.DTOs
{
    public class ElementDTO
    {
        [Display(Name = "Grouped By")]
        public string GroupedBy { get; set; } = string.Empty;

        [Display(Name = "Element Order")]
        public string ElementOrder { get; set; } = string.Empty;

        public string Element { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;

        [Display(Name = "Year Month")]
        public string YearMon { get; set; } = string.Empty;

        [Display(Name = "Year Month")]
        public string YearMonth { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        [Display(Name = "Photos")]
        public string PhotoCount { get; set; } = string.Empty;
    }
}
