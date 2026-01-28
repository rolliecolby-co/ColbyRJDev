namespace ColbyRJ.Repository
{
    public class StoryRepository : IStoryRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IFileUpload _fileUpload;
        private readonly IUtilityRepository _utility;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public StoryRepository(
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

        public async Task<string> Create(StoryCreateDTO storyDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var category = "New";
            var groupedBy = "New";

            var yearStr = storyDTO.YearInt.ToString();
            var monStr = storyDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            var story = new Story
            {
                Title = storyDTO.Title,

                Category = "New",
                GroupedBy = "New",
                YearStr = yearStr,
                MonStr = monStr,
                YearMon = yearMon,
                YearMonth = yearMonth,
                Element = "Story",
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                DateUpdated = DateTime.Now
            };

            ctx.Stories.Add(story);
            await ctx.SaveChangesAsync();

            Random rnd = new Random();
            var key = story.Id.ToString() + "-" + rnd.Next(story.Id * 7, story.Id * 123).ToString();
            story.Key = key;

            ctx.Stories.Update(story);
            await ctx.SaveChangesAsync();

            return "id-" + story.Id.ToString();
        }

        public async Task<int> Delete(int storyId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var story = await ctx.Stories
                .Include(s => s.Photos)
                .Include(s => s.Chapters)
                .FirstOrDefaultAsync(t => t.Id == storyId);

            if (story.AudioUrl.Length > 1)
            {
                _fileUpload.DeleteFile(story.AudioFilename, "storyAudio");
            }

            if (story.Photos != null && story.Photos.Count > 0)
            {
                foreach (var item in story.Photos)
                {
                    var photoUrl = item.PhotoUrl.ToLower();
                    var photoName = photoUrl.Replace($"storyphotos/", "");
                    _fileUpload.DeleteFile(photoName, "storyPhotos");
                }
            }

            if (story.Chapters != null && story.Chapters.Count > 0)
            {
                foreach (var chap in story.Chapters)
                {
                    var chapter = await ctx.StoryChapters
                                    .Include(c => c.Photos)
                                    .FirstOrDefaultAsync(c => c.Id == chap.Id);

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
                }
            }

            ctx.Stories.Remove(story);
            return await ctx.SaveChangesAsync();
        }

        public async Task<StoryDTO> GetStory(int storyId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var story = await ctx.Stories
                .Include(x => x.Photos.OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate))
                .Include(s => s.Chapters.OrderBy(c => c.ChapterDate))
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == storyId);

            if (story == null)
            {
                return null;
            }

            var storyDTO = _mapper.Map<Story, StoryDTO>(story);

            storyDTO.YearInt = Convert.ToInt32(storyDTO.YearStr);
            storyDTO.Decade = storyDTO.YearStr.Substring(0, 3) + "0s";

            return storyDTO;
        }

        public async Task<List<StoryDTO>> GetStories()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var stories = await ctx.Stories
                .Include(x => x.Photos.OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate))
                .Include(s => s.Chapters.OrderBy(c => c.ChapterDate))
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .Where(q => q.OwnerEmail == appUser.Email || appUser.Role == "Admin")
                .AsNoTracking()
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.Title)
            .ToListAsync();

            var storiesDTO = _mapper.Map<List<Story>, List<StoryDTO>>(stories);

            storiesDTO.ForEach(s =>
            {
                if (s.Active)
                {
                    s.ActiveStr = "yes";
                }

                if (s.Photos.Count > 0)
                {
                    s.PhotoCount = s.Photos.Count.ToString();
                }

                if (s.Chapters.Count > 0)
                {
                    s.ChapterCount = s.Chapters.Count.ToString();
                }

                if (s.Comments.Count > 0)
                {
                    s.CommentCount = s.Comments.Count.ToString();
                }

                s.Decade = s.YearStr.Substring(0, 3) + "0s";
            });

            return storiesDTO;
        }

        public async Task<StoryDTO> Update(StoryDTO storyDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var story = await ctx.Stories
                .FirstOrDefaultAsync(t => t.Id == storyDTO.Id);

            var yearStr = storyDTO.YearInt.ToString();
            var monStr = storyDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            story.Title = storyDTO.Title;
            story.YearStr = yearStr;
            story.MonStr = monStr;
            story.YearMon = yearMon;
            story.YearMonth = yearMonth;
            story.Owner = storyDTO.Owner;
            story.OwnerEmail = storyDTO.OwnerEmail;
            story.DateUpdated = DateTime.Now;
            story.Body = storyDTO.Body;
            story.Active = storyDTO.Active;
            story.AudioFilename = storyDTO.AudioFilename;
            story.AudioUrl = storyDTO.AudioUrl;

            var updatedStory = ctx.Stories.Update(story);
            await ctx.SaveChangesAsync();

            return _mapper.Map<Story, StoryDTO>(story);
        }

        public async Task<string> UpdateGroupBy(GroupByEditDTO groupByDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var story = await ctx.Stories
                .FirstOrDefaultAsync(t => t.Id == groupByDTO.Id);

            var category = groupByDTO.Category;
            var section = groupByDTO.Section.ToString();
            var topic = groupByDTO.Topic.ToString();
            var groupedBy = await _utility.GetGroupedBy(category, section, topic);

            story.Category = groupByDTO.Category;
            story.Section = groupByDTO.Section.ToString();
            story.Topic = groupByDTO.Topic.ToString();
            story.GroupedBy = groupedBy;
            story.DateUpdated = DateTime.Now;

            ctx.Stories.Update(story);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<StoryDTO> GetStoryByKey(string key)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var story = await ctx.Stories
                .Include(x => x.Photos.OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate))
                .Include(s => s.Chapters.OrderBy(c => c.ChapterDate))
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Key == key);

            if (story == null)
            {
                return null;
            }

            var storyDTO = _mapper.Map<Story, StoryDTO>(story);

            storyDTO.YearInt = Convert.ToInt32(storyDTO.YearStr);
            storyDTO.Decade = storyDTO.YearStr.Substring(0, 3) + "0s";

            return storyDTO;
        }

        public async Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var story = await ctx.Stories
                .FirstOrDefaultAsync(t => t.Id == yearMonDTO.Id);

            var yearStr = yearMonDTO.YearInt.ToString();
            var monStr = yearMonDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            story.YearStr = yearStr;
            story.MonStr = monStr;
            story.YearMon = yearMon;
            story.YearMonth = yearMonth;
            story.DateUpdated = DateTime.Now;

            ctx.Stories.Update(story);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<List<StoryDTO>> GetActiveStories()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var stories = await ctx.Stories
                .Include(x => x.Photos.OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate))
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .Include(s => s.Chapters.OrderBy(c => c.YearMon))
                    .ThenInclude(c => c.Photos)
                .Include(s => s.Chapters.OrderBy(c => c.YearMon))
                    .ThenInclude(c => c.Comments)
                .Where(q => q.Active == true)
                .AsNoTracking()
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.YearMon).ThenBy(s => s.Title)
            .ToListAsync();

            var storiesDTO = _mapper.Map<List<Story>, List<StoryDTO>>(stories);

            storiesDTO.ForEach(s =>
            {
                if (s.Photos.Count > 0)
                {
                    s.PhotoCount = s.Photos.Count.ToString();
                }

                if (s.Chapters.Count > 0)
                {
                    s.ChapterCount = s.Chapters.Count.ToString();

                    foreach (var item in s.Chapters)
                    {
                        item.PhotoCount = item.Photos.Count.ToString();
                    }
                }

                if (s.Comments.Count > 0)
                {
                    s.CommentCount = s.Comments.Count.ToString();

                    foreach (var item in s.Chapters)
                    {
                        item.CommentCount = item.Comments.Count.ToString();
                    }
                }

                s.Decade = s.YearStr.Substring(0, 3) + "0s";
            });

            return storiesDTO;
        }
    }
}
