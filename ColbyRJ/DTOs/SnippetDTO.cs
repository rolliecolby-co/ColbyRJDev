namespace ColbyRJ.DTOs
{
    public class SnippetCreateDTO : BaseCreateDTO
    {
    }

    public class SnippetDTO : BaseDTO
    {
        [Required]
        [RemarksValidator(Remarks = "The Snippet")]
        public string Body { get; set; } = string.Empty;

        public string ActiveStr { get; set; } = string.Empty;

        public string AudioFilename { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;

        [Display(Name = "With Audio")]
        public string WithAudio { get; set; } = string.Empty;
        [Display(Name = "Photos")]
        public string PhotoCount { get; set; } = string.Empty;
        [Display(Name = "Comments")]
        public string CommentCount { get; set; } = string.Empty;

        public ICollection<SnippetPhotoDTO>? Photos { get; set; }
        public ICollection<SnippetCommentDTO>? Comments { get; set; }

        public List<string> PhotoUrls { get; set; }
    }
}
