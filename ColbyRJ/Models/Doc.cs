namespace ColbyRJ.Models
{
    public class Doc : BaseEntity
    {
        public string Remarks { get; set; } = string.Empty;

        public ICollection<DocPdf>? Pdfs { get; set; }
        public ICollection<DocComment>? Comments { get; set; }
    }
}
