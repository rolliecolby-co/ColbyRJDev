namespace ColbyRJ.DTOs
{
    public class HintDTO
    {
        public int Id { get; set; }

        [Required]
        public string Key { get; set; } = string.Empty;

        [Display(Name = "Order #")]
        public int OrderBy { get; set; } = 99;

        [Required]
        public string Title { get; set; } = string.Empty;
        public string Page { get; set; } = string.Empty;

        [Required]
        [RemarksValidator(Remarks = "The Hint")]
        public string Value { get; set; } = string.Empty;
    }
}
