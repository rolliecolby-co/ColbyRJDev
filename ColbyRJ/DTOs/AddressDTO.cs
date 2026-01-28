namespace ColbyRJ.DTOs
{
    public class AddressDTO
    {
        public int Id { get; set; }
        [Required]
        public string Who { get; set; } = string.Empty;

        
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Start Month Year")]
        public string StartDateStr { get; set; }


        [Display(Name = "Year")]
        public string YearStr { get; set; } = string.Empty;

        [Display(Name = "Year")]
        [Range(1910, 2050, ErrorMessage = "Please enter valid Year")]
        public int YearInt { get; set; } = DateTime.Now.Year;
        [Display(Name = "Mon")]
        public string MonStr { get; set; } = string.Empty;

        [Display(Name = "Year Mon")]
        public string YearMon { get; set; } = string.Empty;
        [Display(Name = "Year Month")]
        public string YearMonth { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Home Address")]
        public string HomeAddress { get; set; } = string.Empty;

        public string Remarks { get; set; } = string.Empty;

        [Display(Name = "Remarks")]
        public string WithRemarks { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;

        [Display(Name = "Owner Email")]
        public string OwnerEmail { get; set; } = string.Empty;
        [Display(Name = "Photos")]
        public string PhotoCount { get; set; } = string.Empty;
        [Display(Name = "Comments")]
        public string CommentCount { get; set; } = string.Empty;

        public ICollection<AddressPhotoDTO>? Photos { get; set; }
        public ICollection<AddressCommentDTO>? Comments { get; set; }

        public List<string> PhotoUrls { get; set; }
    }
}
