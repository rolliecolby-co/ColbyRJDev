namespace ColbyRJ.DTOs
{
    public class DocPdfDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        [Display(Name = "Order #")]
        public int? OrderBy { get; set; }

        public DateTime? PdfDate { get; set; }
        public string PdfDateStr
        {
            get
            {
                if (PdfDate != null)
                {
                    var pdfDate = Convert.ToDateTime(PdfDate);
                    var pdfDateStr = pdfDate.ToString("MMM yyyy");
                    return pdfDateStr;
                }
                else
                {
                    return "";
                }
            }
            set { }
        }
        public string PdfUrl { get; set; } = string.Empty;

        public DateTime DateUpdated { get; set; }
        public int DocId { get; set; }
        public DocDTO Doc { get; set; }
    }
}
