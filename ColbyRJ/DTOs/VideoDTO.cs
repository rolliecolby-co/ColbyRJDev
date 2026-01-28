namespace ColbyRJ.DTOs
{
    public class VideoCreateDTO : BaseCreateDTO
    {
    }

    public class VideoDTO : BaseDTO
    {
        [Required]
        [RemarksValidator(Remarks = "The Video")]
        public string Remarks { get; set; } = string.Empty;

        public string ActiveStr { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Video File")]
        public string VideoFilename { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;

        [Display(Name = "Comments")]
        public string CommentCount { get; set; } = string.Empty;

        public ICollection<VideoCommentDTO>? Comments { get; set; }
    }
}
