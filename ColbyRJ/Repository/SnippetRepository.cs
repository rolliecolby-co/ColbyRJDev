namespace ColbyRJ.Repository
{
    public class SnippetRepository : ISnippetRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IFileUpload _fileUpload;
        private readonly IUtilityRepository _utility;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public SnippetRepository(
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

        public async Task<string> Create(SnippetCreateDTO snippetDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var category = "New";
            var groupedBy = "New";

            var yearStr = snippetDTO.YearInt.ToString();
            var monStr = snippetDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            var snippet = new Snippet
            {
                Title = snippetDTO.Title,

                Category = "New",
                GroupedBy = "New",
                YearStr = yearStr,
                MonStr = monStr,
                YearMon = yearMon,
                YearMonth = yearMonth,
                Element = "Snippet",
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                DateUpdated = DateTime.Now
            };

            ctx.Snippets.Add(snippet);
            await ctx.SaveChangesAsync();

            Random rnd = new Random();
            var key = snippet.Id.ToString() + "-" + rnd.Next(snippet.Id * 7, snippet.Id * 123).ToString();

            snippet.Key = key;

            ctx.Snippets.Update(snippet);
            await ctx.SaveChangesAsync();

            return "id-" + snippet.Id.ToString();
        }

        public async Task<int> Delete(int snippetId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var snippet = await ctx.Snippets
                .Include(s => s.Photos)
                .FirstOrDefaultAsync(t => t.Id == snippetId);

            if (snippet.AudioUrl.Length > 1)
            {
                _fileUpload.DeleteFile(snippet.AudioFilename, "snippetAudio");
            }

            if (snippet.Photos != null && snippet.Photos.Count > 0)
            {
                foreach (var item in snippet.Photos)
                {
                    var photoUrl = item.PhotoUrl.ToLower();
                    var photoName = photoUrl.Replace($"snippetphotos/", "");
                    _fileUpload.DeleteFile(photoName, "snippetPhotos");
                }
            }

            ctx.Snippets.Remove(snippet);
            return await ctx.SaveChangesAsync();
        }

        public async Task<List<SnippetDTO>> GetActiveSnippets()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var snippets = await ctx.Snippets
                .Include(x => x.Photos.OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate))
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .Where(q => q.Active == true)
                .AsNoTracking()
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.YearMon).ThenBy(s => s.Title)
            .ToListAsync();

            var snippetsDTO = _mapper.Map<List<Snippet>, List<SnippetDTO>>(snippets);

            snippetsDTO.ForEach(s =>
            {
                if (s.Photos.Count > 0)
                {
                    s.PhotoCount = s.Photos.Count.ToString();
                }

                if (s.Comments.Count > 0)
                {
                    s.CommentCount = s.Comments.Count.ToString();
                }

                if (s.AudioFilename.Length > 0)
                {
                    s.WithAudio = "Yes";
                }

                s.Decade = s.YearStr.Substring(0, 3) + "0s";
            });

            return snippetsDTO;
        }

        public async Task<SnippetDTO> GetSnippet(int snippetId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var snippet = await ctx.Snippets
                .Include(x => x.Photos.OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate))
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == snippetId);

            if (snippet == null)
            {
                return null;
            }

            var snippetDTO = _mapper.Map<Snippet, SnippetDTO>(snippet);

            snippetDTO.YearInt = Convert.ToInt32(snippetDTO.YearStr);
            snippetDTO.Decade = snippetDTO.YearStr.Substring(0, 3) + "0s";

            return snippetDTO;
        }

        public async Task<SnippetDTO> GetSnippetByKey(string key)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var snippet = await ctx.Snippets
                .Include(x => x.Photos.OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate))
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Key == key);

            if (snippet == null)
            {
                return null;
            }

            var snippetDTO = _mapper.Map<Snippet, SnippetDTO>(snippet);

            snippetDTO.YearInt = Convert.ToInt32(snippetDTO.YearStr);
            snippetDTO.Decade = snippetDTO.YearStr.Substring(0, 3) + "0s";
            return snippetDTO;
        }

        public async Task<List<SnippetDTO>> GetSnippets()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var snippets = await ctx.Snippets
                .Include(x => x.Photos.OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate))
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .Where(q => q.OwnerEmail == appUser.Email || appUser.Role == "Admin")
                .AsNoTracking()
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.Title)
            .ToListAsync();

            var snippetsDTO = _mapper.Map<List<Snippet>, List<SnippetDTO>>(snippets);

            snippetsDTO.ForEach(s =>
            {
                if (s.Active)
                {
                    s.ActiveStr = "yes";
                }

                if (s.Photos.Count > 0)
                {
                    s.PhotoCount = s.Photos.Count.ToString();
                }

                if (s.Comments.Count > 0)
                {
                    s.CommentCount = s.Comments.Count.ToString();
                }

                if (s.AudioFilename.Length > 0)
                {
                    s.WithAudio = "Audio";
                }

                s.Decade = s.YearStr.Substring(0, 3) + "0s";
            });

            return snippetsDTO;
        }

        public async Task<SnippetDTO> Update(SnippetDTO snippetDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var snippet = await ctx.Snippets
                .FirstOrDefaultAsync(t => t.Id == snippetDTO.Id);

            snippet.Title = snippetDTO.Title;
            snippet.Owner = snippetDTO.Owner;
            snippet.OwnerEmail = snippetDTO.OwnerEmail;
            snippet.DateUpdated = DateTime.Now;
            snippet.Body = snippetDTO.Body;
            snippet.Active = snippetDTO.Active;
            snippet.AudioFilename = snippetDTO.AudioFilename;
            snippet.AudioUrl = snippetDTO.AudioUrl;

            ctx.Snippets.Update(snippet);
            await ctx.SaveChangesAsync();

            return _mapper.Map<Snippet, SnippetDTO>(snippet);
        }

        public async Task<string> UpdateGroupBy(GroupByEditDTO groupByDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var snippet = await ctx.Snippets
                .FirstOrDefaultAsync(t => t.Id == groupByDTO.Id);

            var category = groupByDTO.Category;
            var section = groupByDTO.Section.ToString();
            var topic = groupByDTO.Topic.ToString();
            var groupedBy = await _utility.GetGroupedBy(category, section, topic);

            snippet.Category = groupByDTO.Category;
            snippet.Section = groupByDTO.Section.ToString();
            snippet.Topic = groupByDTO.Topic.ToString();
            snippet.GroupedBy = groupedBy;
            snippet.DateUpdated = DateTime.Now;

            ctx.Snippets.Update(snippet);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var snippet = await ctx.Snippets
                .FirstOrDefaultAsync(t => t.Id == yearMonDTO.Id);

            var yearStr = yearMonDTO.YearInt.ToString();
            var monStr = yearMonDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            snippet.YearStr = yearStr;
            snippet.MonStr = monStr;
            snippet.YearMon = yearMon;
            snippet.YearMonth = yearMonth;
            snippet.DateUpdated = DateTime.Now;

            ctx.Snippets.Update(snippet);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
