namespace ColbyRJ.DTOs
{
    public class PhotoAlbumCreateDTO : BaseCreateDTO
    {
    }

    public class PhotoAlbumDTO : BaseDTO
    {
        [Required]
        [RemarksValidator(Remarks = "The Photo Album")]
        public string Remarks { get; set; } = string.Empty;

        public string ActiveStr { get; set; } = string.Empty;

        [Display(Name = "Photos")]
        public string PhotoCount { get; set; } = string.Empty;
        [Display(Name = "Comments")]
        public string CommentCount { get; set; } = string.Empty;

        public ICollection<PhotoAlbumPhotoDTO>? Photos { get; set; }
        public ICollection<PhotoAlbumCommentDTO>? Comments { get; set; }

        public List<string> PhotoUrls { get; set; }
    }
}
