namespace ColbyRJ.Repository
{
    public class StoryChapterRepository : IStoryChapterRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IFileUpload _fileUpload;
        private readonly IUtilityRepository _utility;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public StoryChapterRepository(
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

        public async Task<string> Create(StoryChapterCreateDTO chapterDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var yearStr = chapterDTO.YearInt.ToString();
            var monStr = chapterDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            var chapter = new StoryChapter
            {
                StoryId = chapterDTO.StoryId,
                Title = chapterDTO.Title,
                //ChapterDate = chapterDTO.ChapterDate,
                YearStr = yearStr,
                MonStr = monStr,
                YearMon = yearMon,
                YearMonth = yearMonth,
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                DateUpdated = DateTime.Now
            };

            ctx.StoryChapters.Add(chapter);
            await ctx.SaveChangesAsync();

            Random rnd = new Random();
            var key = chapter.Id.ToString() + "-" + rnd.Next(chapter.Id * 7, chapter.Id * 123).ToString();
            chapter.Key = key;

            ctx.StoryChapters.Update(chapter);
            await ctx.SaveChangesAsync();

            return "id-" + chapter.Id.ToString();
        }

        public async Task<int> Delete(int chapterId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var chapter = await ctx.StoryChapters
                .Include(c => c.Photos)
                .FirstOrDefaultAsync(t => t.Id == chapterId);

            if (chapter.AudioUrl.Length > 1)
            {
                _fileUpload.DeleteFile(chapter.AudioFilename, "storyChapterAudio");
            }

            if (chapter.Photos != null && chapter.Photos.Count > 0)
            {
                foreach (var item in chapter.Photos)
                {
                    var photoUrl = item.PhotoUrl.ToLower();
                    var photoName = photoUrl.Replace($"storychapterphotos/", "");
                    _fileUpload.DeleteFile(photoName, "storyChapterPhotos");
                }
            }

            ctx.StoryChapters.Remove(chapter);
            return await ctx.SaveChangesAsync();
        }

        public async Task<StoryChapterDTO> GetChapter(int chapterId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            try
            {
                var chapter = await ctx.StoryChapters
                    .Include(q => q.Story)
                    .Include(x => x.Photos.OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate))
                    .Include(s => s.Comments.OrderByDescending(a => a.Id))
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == chapterId);

                if (chapter == null)
                {
                    return null;
                }

                var chapterDTO = _mapper.Map<StoryChapter, StoryChapterDTO>(chapter);

                chapterDTO.YearInt = Convert.ToInt32(chapterDTO.YearStr);

                return chapterDTO;
            }
            catch (Exception ex)
            {

            }
            return new StoryChapterDTO();
        }

        public async Task<List<StoryChapterDTO>> GetChapters()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var chapters = await ctx.StoryChapters
                .Include(q => q.Story)
                .Include(x => x.Photos.OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate))
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .OrderBy(s => s.YearMon).ThenBy(s => s.Title)
            .ToListAsync();

            var chaptersDTO = _mapper.Map<List<StoryChapter>, List<StoryChapterDTO>>(chapters);

            chaptersDTO.ForEach(s =>
            {
                if (s.Active)
                {
                    s.ActiveStr = "active";
                }

                if (s.Photos.Count > 0)
                {
                    s.PhotoCount = s.Photos.Count.ToString();
                }

                if (s.Comments.Count > 0)
                {
                    s.CommentCount = s.Comments.Count.ToString();
                }
            });

            return chaptersDTO;
        }

        public async Task<StoryChapterDTO> GetStoryChapterByKey(string key)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var chapter = await ctx.StoryChapters
                .Include(q => q.Story)
                .Include(x => x.Photos.OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate))
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Key == key);

            if (chapter == null)
            {
                return null;
            }

            var chapterDTO = _mapper.Map<StoryChapter, StoryChapterDTO>(chapter);

            return chapterDTO;
        }

        public async Task<StoryChapterDTO> Update(StoryChapterDTO chapterDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var chapter = await ctx.StoryChapters
                .FirstOrDefaultAsync(t => t.Id == chapterDTO.Id);

            var yearStr = chapterDTO.YearInt.ToString();
            var monStr = chapterDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            chapter.Title = chapterDTO.Title;
            chapter.Body = chapterDTO.Body;
            //chapter.ChapterDate = chapterDTO.ChapterDate;
            chapter.YearStr = yearStr;
            chapter.MonStr = monStr;
            chapter.YearMon = yearMon;
            chapter.YearMonth = yearMonth;
            chapter.Active = chapterDTO.Active;
            chapter.DateUpdated = DateTime.Now;
            chapter.AudioFilename = chapterDTO.AudioFilename;
            chapter.AudioUrl = chapterDTO.AudioUrl;

            ctx.StoryChapters.Update(chapter);
            await ctx.SaveChangesAsync();

            return _mapper.Map<StoryChapter, StoryChapterDTO>(chapter);
        }

        public async Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var chapter = await ctx.StoryChapters
                .FirstOrDefaultAsync(t => t.Id == yearMonDTO.Id);

            var yearStr = yearMonDTO.YearInt.ToString();
            var monStr = yearMonDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            chapter.YearStr = yearStr;
            chapter.MonStr = monStr;
            chapter.YearMon = yearMon;
            chapter.YearMonth = yearMonth;
            chapter.DateUpdated = DateTime.Now;

            ctx.StoryChapters.Update(chapter);
            await ctx.SaveChangesAsync();

            return "ok";
        }

    }
}
