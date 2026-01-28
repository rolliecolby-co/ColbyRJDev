namespace ColbyRJ.Models
{
    public class DocPdf
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public int? OrderBy { get; set; }


        [Column(TypeName = "Date")]
        public DateTime? PdfDate { get; set; }
        public string PdfUrl { get; set; } = string.Empty;

        [Column(TypeName = "Date")]
        public DateTime DateUpdated { get; set; }
        public int DocId { get; set; }
        public Doc Doc { get; set; }
    }
}
