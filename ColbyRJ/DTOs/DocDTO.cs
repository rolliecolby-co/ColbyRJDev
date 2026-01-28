namespace ColbyRJ.DTOs
{
    public class DocCreateDTO : BaseCreateDTO
    {
    }
    public class DocDTO : BaseDTO
    {
        [Required]
        [RemarksValidator(Remarks = "The Document")]
        public string Remarks { get; set; } = string.Empty;

        public string ActiveStr { get; set; } = string.Empty;

        [Display(Name = "Documents")]
        public string PdfCount { get; set; } = string.Empty;
        [Display(Name = "Comments")]
        public string CommentCount { get; set; } = string.Empty;

        public ICollection<DocPdfDTO>? Pdfs { get; set; }
        public ICollection<DocCommentDTO>? Comments { get; set; }

        public List<string> PdfUrls { get; set; }
    }
}
