namespace ColbyRJ.DTOs
{
    public class JobDTO
    {
        public int Id { get; set; }
        
        public string Who { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "StartMonth Year")]
        public string StartDateStr { get; set; }


        [Display(Name = "Job Year")]
        public string YearStr { get; set; } = string.Empty;

        [Display(Name = "Job Year")]
        [Range(1910, 2050, ErrorMessage = "Please enter valid Year")]
        public int YearInt { get; set; } = DateTime.Now.Year;

        [Display(Name = "Mon")]
        public string MonStr { get; set; } = string.Empty;
        [Display(Name = "Year Mon")]
        public string YearMon { get; set; } = string.Empty;
        [Display(Name = "Job Start")]
        public string YearMonth { get; set; } = string.Empty;

        [Required]
        public string Employer { get; set; } = string.Empty;
        [Required]
        public string Position { get; set; } = string.Empty;
        public string EmployerPosition
        {
            get
            {
                if (Employer?.Length > 1 && Position?.Length > 1)
                {
                    return Employer + " / " + Position;
                }
                else if (Employer?.Length > 1)
                {
                    return Employer;
                }
                else
                {
                    return "";
                }
            }
            set { }
        }

        public string Remarks { get; set; } = string.Empty;

        [Display(Name = "Remarks")]
        public string WithRemarks { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;

        public DateTime DateUpdated { get; set; }
        [Display(Name = "Comments")]
        public string CommentCount { get; set; } = string.Empty;

        public ICollection<JobCommentDTO>? Comments { get; set; }
    }
}
