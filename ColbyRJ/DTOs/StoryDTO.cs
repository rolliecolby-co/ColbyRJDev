namespace ColbyRJ.DTOs
{
    public class StoryCreateDTO : BaseCreateDTO
    {
    }
    public class StoryDTO : BaseDTO
    {
        [Required]
        [RemarksValidator(Remarks = "The Story")]
        public string Body { get; set; } = string.Empty;

        public string ActiveStr { get; set; } = string.Empty;
        [Display(Name = "Photos")]
        public string PhotoCount { get; set; } = string.Empty;
        [Display(Name = "Chapters")]
        public string ChapterCount { get; set; } = string.Empty;
        [Display(Name = "Comments")]
        public string CommentCount { get; set; } = string.Empty;

        public string AudioFilename { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;

        [Display(Name = "With Audio")]
        public string WithAudio { get; set; } = string.Empty;

        public ICollection<StoryPhotoDTO>? Photos { get; set; }
        public ICollection<StoryChapterDTO>? Chapters { get; set; }
        public ICollection<StoryCommentDTO>? Comments { get; set; }

        public List<string> PhotoUrls { get; set; }
    }
}
