namespace ColbyRJ.Repository
{
    public class DocPdfRepository : IDocPdfRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IFileUpload _fileUpload;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public DocPdfRepository(
            IDbContextFactory<ApplicationDbContext> ctxFactory,
            IMapper mapper,
            IFileUpload fileUpload,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContext)
        {
            _ctxFactory = ctxFactory;
            _mapper = mapper;
            _fileUpload = fileUpload;
            _userManager = userManager;
            _httpContext = httpContext;
        }

        public async Task<string> Create(DocPdfDTO pdfDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var photo = new DocPdf
            {
                Title = pdfDTO.Title,
                DocId = pdfDTO.DocId,
                PdfDate = pdfDTO.PdfDate,
                PdfUrl = pdfDTO.PdfUrl,
                OrderBy = 99,
                DateUpdated = DateTime.Now
            };

            await ctx.DocPdfs.AddAsync(photo);
            await ctx.SaveChangesAsync();

            return "id-" + photo.Id.ToString();
        }

        public async Task<int> Delete(int pdfId)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var pdf = await ctx.DocPdfs.FirstOrDefaultAsync(x => x.Id == pdfId);


            var pdfUrl = pdf.PdfUrl;
            var pdfName = pdfUrl.Replace($"docPdfs/", "");

            var result = _fileUpload.DeleteFile(pdfName, "docPdfs");

            ctx.DocPdfs.Remove(pdf);
            return await ctx.SaveChangesAsync();
        }

        public async Task<int> DeletePdfByPdfUrl(string pdfUrl)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var allPdfs = await ctx.DocPdfs.FirstOrDefaultAsync
                                (x => x.PdfUrl.ToLower() == pdfUrl.ToLower());
            if (allPdfs == null)
            {
                return 0;
            }
            ctx.DocPdfs.Remove(allPdfs);
            return await ctx.SaveChangesAsync();
        }

        public async Task<DocPdfDTO> GetPdf(int pdfId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var pdf = await ctx.DocPdfs
                .Include(q => q.Doc)
                .AsNoTracking()
               .FirstOrDefaultAsync(q => q.Id == pdfId);

            var pdfDTO = _mapper.Map<DocPdf, DocPdfDTO>(pdf);

            try
            {
                DateTime pdfDate = (DateTime)pdfDTO.PdfDate;
                pdfDTO.PdfDateStr = pdfDate.ToString("M/d/yyyy");
            }
            catch { }

            return pdfDTO;
        }

        public async Task<List<DocPdfDTO>> GetPdfs(int docId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var pdfs = await ctx.DocPdfs
                .Include(q => q.Doc)
                .Where(q => q.DocId == docId)
              .OrderBy(a => a.OrderBy).ThenBy(b => b.PdfDate)
              .AsNoTracking().ToListAsync();

            var pdfsDTO = _mapper.Map<List<DocPdf>, List<DocPdfDTO>>(pdfs);

            pdfsDTO.ForEach(p =>
            {
                try
                {
                    DateTime pdfDate = (DateTime)p.PdfDate;
                    p.PdfDateStr = pdfDate.ToString("M/d/yyyy");
                }
                catch { }
            });

            return pdfsDTO;
        }

        public async Task<string> Update(DocPdfDTO pdfDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var pdf = await ctx.DocPdfs.FirstOrDefaultAsync(q => q.Id == pdfDTO.Id);

            pdfDTO.Title = pdfDTO.Title;
            pdf.OrderBy = pdfDTO.OrderBy;
            pdf.Title = pdfDTO.Title;
            pdf.PdfDate = pdfDTO.PdfDate;
            pdf.DateUpdated = DateTime.Now;

            ctx.DocPdfs.Update(pdf);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
