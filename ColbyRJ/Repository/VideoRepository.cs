namespace ColbyRJ.Repository
{
    public class VideoRepository : IVideoRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IFileUpload _fileUpload;
        private readonly IUtilityRepository _utility;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public VideoRepository(
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

        public async Task<string> Create(VideoCreateDTO videoDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var category = "New";
            var groupedBy = "New";

            var yearStr = videoDTO.YearInt.ToString();
            var monStr = videoDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            var video = new Video
            {
                Title = videoDTO.Title,

                Category = "New",
                GroupedBy = "New",
                YearStr = yearStr,
                MonStr = monStr,
                YearMon = yearMon,
                YearMonth = yearMonth,
                Element = "Video",
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                DateUpdated = DateTime.Now
            };

            ctx.Videos.Add(video);
            await ctx.SaveChangesAsync();

            Random rnd = new Random();
            var key = video.Id.ToString() + "-" + rnd.Next(video.Id * 7, video.Id * 123).ToString();
            video.Key = key;

            ctx.Videos.Update(video);
            await ctx.SaveChangesAsync();

            return "id-" + video.Id.ToString();
        }

        public async Task<int> Delete(int videoId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var video = await ctx.Videos.FirstOrDefaultAsync(t => t.Id == videoId);

            if (video.VideoUrl.Length > 1)
            {
                _fileUpload.DeleteFile(video.VideoFilename, "videos");
            }

            ctx.Videos.Remove(video);
            return await ctx.SaveChangesAsync();
        }

        public async Task<List<VideoDTO>> GetActiveVideos()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var videos = await ctx.Videos
                .Include(v => v.Comments.OrderByDescending(a => a.Id))
                .Where(q => q.Active == true)
                .AsNoTracking()
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.YearMon).ThenBy(s => s.Title)
            .ToListAsync();

            var videosDTO = _mapper.Map<List<Video>, List<VideoDTO>>(videos);

            videosDTO.ForEach(v =>
            {
                if (v.Comments.Count > 0)
                {
                    v.CommentCount = v.Comments.Count.ToString();
                }

                v.Decade = v.YearStr.Substring(0, 3) + "0s";
            });

            return videosDTO;
        }

        public async Task<VideoDTO> GetVideo(int videoId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var video = await ctx.Videos
                .Include(v => v.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == videoId);

            if (video == null)
            {
                return null;
            }

            var videoDTO = _mapper.Map<Video, VideoDTO>(video);

            videoDTO.YearInt = Convert.ToInt32(videoDTO.YearStr);
            videoDTO.Decade = videoDTO.YearStr.Substring(0, 3) + "0s";

            return videoDTO;
        }

        public async Task<VideoDTO> GetVideoByKey(string key)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var video = await ctx.Videos
                .Include(v => v.Comments.OrderByDescending(a => a.Id)).Include(v => v.Comments)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Key == key);

            if (video == null)
            {
                return null;
            }

            var videoDTO = _mapper.Map<Video, VideoDTO>(video);

            videoDTO.YearInt = Convert.ToInt32(videoDTO.YearStr);
            videoDTO.Decade = videoDTO.YearStr.Substring(0, 3) + "0s";

            return videoDTO;
        }

        public async Task<List<VideoDTO>> GetVideos()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var videos = await ctx.Videos
                .Include(v => v.Comments.OrderByDescending(a => a.Id))
                .Where(q => q.OwnerEmail == appUser.Email || appUser.Role == "Admin")
                .AsNoTracking()
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.Title)
            .ToListAsync();

            var videosDTO = _mapper.Map<List<Video>, List<VideoDTO>>(videos);

            videosDTO.ForEach(v =>
            {
                if (v.Active)
                {
                    v.ActiveStr = "yes";
                }

                if (v.Comments.Count > 0)
                {
                    v.CommentCount = v.Comments.Count.ToString();
                }

                v.Decade = v.YearStr.Substring(0, 3) + "0s";
            });

            return videosDTO;
        }

        public async Task<string> Update(VideoDTO videoDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var video = await ctx.Videos
                .FirstOrDefaultAsync(t => t.Id == videoDTO.Id);

            video.Title = videoDTO.Title;

            video.Owner = videoDTO.Owner;
            video.OwnerEmail = videoDTO.OwnerEmail;
            video.Remarks = videoDTO.Remarks;
            video.VideoFilename = videoDTO.VideoFilename;
            video.VideoUrl = videoDTO.VideoUrl;
            video.Active = videoDTO.Active;
            video.DateUpdated = DateTime.Now;

            ctx.Videos.Update(video);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<string> UpdateGroupBy(GroupByEditDTO groupByDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var video = await ctx.Videos
                .FirstOrDefaultAsync(t => t.Id == groupByDTO.Id);

            var category = groupByDTO.Category;
            var section = groupByDTO.Section.ToString();
            var topic = groupByDTO.Topic.ToString();
            var groupedBy = await _utility.GetGroupedBy(category, section, topic);

            video.Category = groupByDTO.Category;
            video.Section = groupByDTO.Section.ToString();
            video.Topic = groupByDTO.Topic.ToString();
            video.GroupedBy = groupedBy;
            video.DateUpdated = DateTime.Now;

            ctx.Videos.Update(video);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<string> UpdateOwner(OwnerEditDTO ownerDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var video = await ctx.Videos
                .FirstOrDefaultAsync(t => t.Id == ownerDTO.Id);

            video.Owner = ownerDTO.Owner;
            video.OwnerEmail = ownerDTO.OwnerEmail;
            video.DateUpdated = DateTime.Now;

            ctx.Videos.Update(video);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var video = await ctx.Videos
                .FirstOrDefaultAsync(t => t.Id == yearMonDTO.Id);

            var yearStr = yearMonDTO.YearInt.ToString();
            var monStr = yearMonDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            video.YearStr = yearStr;
            video.MonStr = monStr;
            video.YearMon = yearMon;
            video.YearMonth = yearMonth;
            video.DateUpdated = DateTime.Now;

            ctx.Videos.Update(video);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
