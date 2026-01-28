namespace ColbyRJ.Repository
{
    public class DocRepository : IDocRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IFileUpload _fileUpload;
        private readonly IUtilityRepository _utility;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public DocRepository(
            IDbContextFactory<ApplicationDbContext> ctxFactory,
            IMapper mapper,
            IFileUpload fileUpload,
            IUtilityRepository utility,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContext)
        {
            _ctxFactory = ctxFactory;
            _mapper = mapper;
            _fileUpload = fileUpload;
            _utility = utility;
            _userManager = userManager;
            _httpContext = httpContext;
        }

        public async Task<string> Create(DocCreateDTO docDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var category = "New";
            var groupedBy = "New";

            var yearStr = docDTO.YearInt.ToString();
            var monStr = docDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            var doc = new Doc
            {
                Title = docDTO.Title,

                Category = "New",
                GroupedBy = "New",
                YearStr = yearStr,
                MonStr = monStr,
                YearMon = yearMon,
                YearMonth = yearMonth,
                Element = "Doc",
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                DateUpdated = DateTime.Now
            };

            ctx.Docs.Add(doc);
            await ctx.SaveChangesAsync();

            Random rnd = new Random();
            var key = doc.Id.ToString() + "-" + rnd.Next(doc.Id * 7, doc.Id * 123).ToString();

            doc.Key = key;

            ctx.Docs.Update(doc);
            await ctx.SaveChangesAsync();

            return "id-" + doc.Id.ToString();
        }

        public async Task<int> Delete(int docId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var doc = await ctx.Docs
                .Include(s => s.Pdfs)
                .FirstOrDefaultAsync(t => t.Id == docId);

            if (doc.Pdfs != null && doc.Pdfs.Count > 0)
            {
                foreach (var item in doc.Pdfs)
                {
                    var pdfUrl = item.PdfUrl.ToLower();
                    var pdfName = pdfUrl.Replace($"docPdfs/", "");
                    _fileUpload.DeleteFile(pdfName, "docPdfs");
                }
            }

            ctx.Docs.Remove(doc);
            return await ctx.SaveChangesAsync();
        }

        public async Task<List<DocDTO>> GetActiveDocs()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var docs = await ctx.Docs
                .Include(x => x.Pdfs.OrderBy(a => a.OrderBy).ThenBy(b => b.PdfDate))
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .Where(q => q.Active == true)
                .AsNoTracking()
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.YearMon).ThenBy(s => s.Title)
            .ToListAsync();

            var docsDTO = _mapper.Map<List<Doc>, List<DocDTO>>(docs);

            docsDTO.ForEach(s =>
            {
                if (s.Pdfs.Count > 0)
                {
                    s.PdfCount = s.Pdfs.Count.ToString();
                }

                if (s.Comments.Count > 0)
                {
                    s.CommentCount = s.Comments.Count.ToString();
                }

                s.Decade = s.YearStr.Substring(0, 3) + "0s";
            });

            return docsDTO;
        }

        public async Task<DocDTO> GetDoc(int docId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var doc = await ctx.Docs
                .Include(x => x.Pdfs.OrderBy(a => a.OrderBy))
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == docId);

            if (doc == null)
            {
                return null;
            }

            var docDTO = _mapper.Map<Doc, DocDTO>(doc);

            docDTO.YearInt = Convert.ToInt32(docDTO.YearStr);
            docDTO.Decade = docDTO.YearStr.Substring(0, 3) + "0s";

            return docDTO;
        }

        public async Task<DocDTO> GetDocByKey(string key)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var doc = await ctx.Docs
                .Include(x => x.Pdfs.OrderBy(a => a.OrderBy).ThenBy(b => b.PdfDate))
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Key == key);

            if (doc == null)
            {
                return null;
            }

            var docDTO = _mapper.Map<Doc, DocDTO>(doc);

            docDTO.YearInt = Convert.ToInt32(docDTO.YearStr);
            docDTO.Decade = docDTO.YearStr.Substring(0, 3) + "0s";
            return docDTO;
        }

        public async Task<List<DocDTO>> GetDocs()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var docs = await ctx.Docs
                .Include(x => x.Pdfs.OrderBy(a => a.OrderBy).ThenBy(b => b.PdfDate))
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .Where(q => q.OwnerEmail == appUser.Email || appUser.Role == "Admin")
                .AsNoTracking()
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.YearMon).ThenBy(s => s.Title)
            .ToListAsync();

            var docsDTO = _mapper.Map<List<Doc>, List<DocDTO>>(docs);

            docsDTO.ForEach(s =>
            {
                if (s.Active)
                {
                    s.ActiveStr = "yes";
                }

                if (s.Pdfs.Count > 0)
                {
                    s.PdfCount = s.Pdfs.Count.ToString();
                }

                if (s.Comments.Count > 0)
                {
                    s.CommentCount = s.Comments.Count.ToString();
                }

                s.Decade = s.YearStr.Substring(0, 3) + "0s";
            });

            return docsDTO;
        }

        public async Task<DocDTO> Update(DocDTO docDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var doc = await ctx.Docs
                .FirstOrDefaultAsync(t => t.Id == docDTO.Id);

            doc.Title = docDTO.Title;
            doc.Owner = docDTO.Owner;
            doc.OwnerEmail = docDTO.OwnerEmail;
            doc.DateUpdated = DateTime.Now;
            doc.Remarks = docDTO.Remarks;
            doc.Active = docDTO.Active;

            ctx.Docs.Update(doc);
            await ctx.SaveChangesAsync();

            return _mapper.Map<Doc, DocDTO>(doc);
        }

        public async Task<string> UpdateGroupBy(GroupByEditDTO groupByDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var doc = await ctx.Docs
                .FirstOrDefaultAsync(t => t.Id == groupByDTO.Id);

            var category = groupByDTO.Category;
            var section = groupByDTO.Section.ToString();
            var topic = groupByDTO.Topic.ToString();
            var groupedBy = await _utility.GetGroupedBy(category, section, topic);

            doc.Category = groupByDTO.Category;
            doc.Section = groupByDTO.Section.ToString();
            doc.Topic = groupByDTO.Topic.ToString();
            doc.GroupedBy = groupedBy;
            doc.DateUpdated = DateTime.Now;

            ctx.Docs.Update(doc);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var doc = await ctx.Docs
                .FirstOrDefaultAsync(t => t.Id == yearMonDTO.Id);

            var yearStr = yearMonDTO.YearInt.ToString();
            var monStr = yearMonDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            doc.YearStr = yearStr;
            doc.MonStr = monStr;
            doc.YearMon = yearMon;
            doc.YearMonth = yearMonth;
            doc.DateUpdated = DateTime.Now;

            ctx.Docs.Update(doc);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
