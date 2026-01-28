namespace ColbyRJ.DTOs
{
    public class PhotoFolderCreateDTO : BaseCreateDTO
    {
    }

    public class PhotoFolderDTO : BaseDTO
    {
        [Required]
        [RemarksValidator(Remarks = "The Photo Folder")]
        public string Remarks { get; set; } = string.Empty;
        public string ActiveStr { get; set; } = string.Empty;
        public string PathFolder { get; set; } = string.Empty;

        public bool ShowFilename { get; set; } = false;
        [Display(Name = "Photos")]
        public string PhotoCount { get; set; } = string.Empty;
        [Display(Name = "Comments")]
        public string CommentCount { get; set; } = string.Empty;

        public ICollection<PhotoFolderCommentDTO>? Comments { get; set; }
    }
}
