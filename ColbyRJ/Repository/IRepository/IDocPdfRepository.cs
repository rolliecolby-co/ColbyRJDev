namespace ColbyRJ.Repository.IRepository
{
    public interface IDocPdfRepository
    {
        public Task<string> Create(DocPdfDTO pdfDTO);
        public Task<int> Delete(int pdfId);
        public Task<int> DeletePdfByPdfUrl(string pdfUrl);
        public Task<List<DocPdfDTO>> GetPdfs(int docId);
        public Task<DocPdfDTO> GetPdf(int pdfId);
        public Task<string> Update(DocPdfDTO pdfDTO);
    }
}
