namespace ColbyRJ.DTOs
{
    public class StoryChapterCreateDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Display(Name = "Chapter Date")]
        public DateTime? ChapterDate { get; set; }

        [Display(Name = "Chapter Year")]
        public string YearStr { get; set; } = string.Empty;

        [Display(Name = "Chapter Year")]
        [Range(1910, 2050, ErrorMessage = "Please enter valid Year")]
        public int YearInt { get; set; } = DateTime.Now.Year;

        [Display(Name = "Mon")]
        public string MonStr { get; set; } = string.Empty;

        public int StoryId { get; set; }
    }
    public class StoryChapterDTO
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        [RemarksValidator(Remarks = "The Story's Chapter")]
        public string Body { get; set; } = string.Empty;

        [Display(Name = "Chapter Date")]
        public DateTime? ChapterDate { get; set; }
        [Display(Name = "Chapter Date")]
        public string ChapterDateStr
        {
            get
            {
                if (ChapterDate != null)
                {
                    var chapterDate = Convert.ToDateTime(ChapterDate);
                    return chapterDate.ToString("MMM yyyy");
                }
                else
                {
                    return "";
                }
            }
            set { }
        }


        [Display(Name = "Chapter Year")]
        public string YearStr { get; set; } = string.Empty;

        [Display(Name = "Chapter Year")]
        [Range(1910, 2050, ErrorMessage = "Please enter valid Year")]
        public int YearInt { get; set; } = DateTime.Now.Year;

        [Display(Name = "Mon")]
        public string MonStr { get; set; } = string.Empty;
        [Display(Name = "Year Mon")]
        public string YearMon { get; set; } = string.Empty;
        [Display(Name = "Chapter When")]
        public string YearMonth { get; set; } = string.Empty;

        [Display(Name = "")]
        public string Blank { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public bool Active { get; set; } = false;

        public DateTime DateUpdated { get; set; }

        public string ActiveStr { get; set; } = string.Empty;
        [Display(Name = "Photos")]
        public string PhotoCount { get; set; } = string.Empty;
        [Display(Name = "Comments")]
        public string CommentCount { get; set; } = string.Empty;

        public string AudioFilename { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;

        [Display(Name = "With Audio")]
        public string WithAudio { get; set; } = string.Empty;

        public int StoryId { get; set; }
        public StoryDTO Story { get; set; }
        public ICollection<StoryChapterPhotoDTO>? Photos { get; set; }
        public ICollection<StoryChapterCommentDTO>? Comments { get; set; }

        public List<string> PhotoUrls { get; set; }
    }
}
