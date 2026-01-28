namespace ColbyRJ.Repository
{
    public class PhotoAlbumRepository : IPhotoAlbumRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IFileUpload _fileUpload;
        private readonly IUtilityRepository _utility;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public PhotoAlbumRepository(
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

        public async Task<string> Create(PhotoAlbumCreateDTO photoAlbumDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var category = "New";
            var groupedBy = "New";

            var yearStr = photoAlbumDTO.YearInt.ToString();
            var monStr = photoAlbumDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            var photoAlbum = new PhotoAlbum
            {
                Title = photoAlbumDTO.Title,

                Category = "New",
                GroupedBy = "New",
                YearStr = yearStr,
                MonStr = monStr,
                YearMon = yearMon,
                YearMonth = yearMonth,
                Element = "Photo Album",
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                DateUpdated = DateTime.Now
            };

            ctx.PhotoAlbums.Add(photoAlbum);
            await ctx.SaveChangesAsync();

            Random rnd = new Random();
            var key = photoAlbum.Id.ToString() + "-" + rnd.Next(photoAlbum.Id * 7, photoAlbum.Id * 123).ToString();
            photoAlbum.Key = key;

            ctx.PhotoAlbums.Update(photoAlbum);
            await ctx.SaveChangesAsync();

            return "id-" + photoAlbum.Id.ToString();
        }

        public async Task<int> Delete(int photoAlbumId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoAlbum = await ctx.PhotoAlbums
                .Include(a => a.Photos)
                .FirstOrDefaultAsync(t => t.Id == photoAlbumId);

            if (photoAlbum.Photos != null && photoAlbum.Photos.Count > 0)
            {
                foreach (var item in photoAlbum.Photos)
                {
                    var photoUrl = item.PhotoUrl.ToLower();
                    var photoName = photoUrl.Replace($"photoalbumphotos/", "");
                    _fileUpload.DeleteFile(photoName, "photoAlbumPhotos");
                }
            }

            ctx.PhotoAlbums.Remove(photoAlbum);
            return await ctx.SaveChangesAsync();
        }

        public async Task<List<PhotoAlbumDTO>> GetActivePhotoAlbums()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoAlbums = await ctx.PhotoAlbums
                .Include(a => a.Photos.OrderBy(p => p.OrderBy).ThenBy(p => p.PhotoDate))
                .Include(a => a.Comments.OrderByDescending(c => c.Id))
                .Where(q => q.Active == true)
                .AsNoTracking()
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.YearMon).ThenBy(s => s.Title)
            .ToListAsync();

            var photoAlbumsDTO = _mapper.Map<List<PhotoAlbum>, List<PhotoAlbumDTO>>(photoAlbums);

            photoAlbumsDTO.ForEach(s =>
            {
                if (s.Photos.Count > 0)
                {
                    s.PhotoCount = s.Photos.Count.ToString();
                }

                if (s.Comments.Count > 0)
                {
                    s.CommentCount = s.Comments.Count.ToString();
                }

                s.Decade = s.YearStr.Substring(0, 3) + "0s";
            });

            return photoAlbumsDTO;
        }

        public async Task<PhotoAlbumDTO> GetPhotoAlbum(int photoAlbumId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoAlbum = await ctx.PhotoAlbums
                .Include(a => a.Photos.OrderBy(p => p.OrderBy).ThenBy(p => p.PhotoDate))
                .Include(a => a.Comments.OrderByDescending(c => c.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == photoAlbumId);

            if (photoAlbum == null)
            {
                return null;
            }

            var photoAlbumDTO = _mapper.Map<PhotoAlbum, PhotoAlbumDTO>(photoAlbum);

            photoAlbumDTO.YearInt = Convert.ToInt32(photoAlbumDTO.YearStr);
            photoAlbumDTO.Decade = photoAlbumDTO.YearStr.Substring(0, 3) + "0s";

            return photoAlbumDTO;
        }

        public async Task<PhotoAlbumDTO> GetPhotoAlbumByKey(string key)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoAlbum = await ctx.PhotoAlbums
                .Include(a => a.Photos.OrderBy(p => p.OrderBy).ThenBy(p => p.PhotoDate))
                .Include(a => a.Comments.OrderByDescending(c => c.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Key == key);

            if (photoAlbum == null)
            {
                return null;
            }

            var photoAlbumDTO = _mapper.Map<PhotoAlbum, PhotoAlbumDTO>(photoAlbum);

            photoAlbumDTO.YearInt = Convert.ToInt32(photoAlbumDTO.YearStr);
            photoAlbumDTO.Decade = photoAlbumDTO.YearStr.Substring(0, 3) + "0s";

            return photoAlbumDTO;
        }

        public async Task<List<PhotoAlbumDTO>> GetPhotoAlbums()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var photoAlbums = await ctx.PhotoAlbums
                .Include(a => a.Photos.OrderBy(p => p.OrderBy).ThenBy(p => p.PhotoDate))
                .Include(a => a.Comments.OrderByDescending(c => c.Id))
                .Where(q => q.OwnerEmail == appUser.Email || appUser.Role == "Admin")
                .AsNoTracking()
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.Title)
            .ToListAsync();

            var photoAlbumsDTO = _mapper.Map<List<PhotoAlbum>, List<PhotoAlbumDTO>>(photoAlbums);

            photoAlbumsDTO.ForEach(s =>
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

                s.Decade = s.YearStr.Substring(0, 3) + "0s";
            });

            return photoAlbumsDTO;
        }

        public async Task<PhotoAlbumDTO> Update(PhotoAlbumDTO photoAlbumDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoAlbum = await ctx.PhotoAlbums
                .FirstOrDefaultAsync(t => t.Id == photoAlbumDTO.Id);

            photoAlbum.Title = photoAlbumDTO.Title;

            photoAlbum.Owner = photoAlbumDTO.Owner;
            photoAlbum.OwnerEmail = photoAlbumDTO.OwnerEmail;
            photoAlbum.Remarks = photoAlbumDTO.Remarks;
            photoAlbum.Active = photoAlbumDTO.Active;
            photoAlbum.DateUpdated = DateTime.Now;

            var updatedPhotoAlbum = ctx.PhotoAlbums.Update(photoAlbum);
            await ctx.SaveChangesAsync();

            return _mapper.Map<PhotoAlbum, PhotoAlbumDTO>(photoAlbum);
        }

        public async Task<string> UpdateGroupBy(GroupByEditDTO groupByDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoAlbum = await ctx.PhotoAlbums
                .FirstOrDefaultAsync(t => t.Id == groupByDTO.Id);

            var category = groupByDTO.Category;
            var section = groupByDTO.Section.ToString();
            var topic = groupByDTO.Topic.ToString();
            var groupedBy = await _utility.GetGroupedBy(category, section, topic);

            photoAlbum.Category = groupByDTO.Category;
            photoAlbum.Section = groupByDTO.Section.ToString();
            photoAlbum.Topic = groupByDTO.Topic.ToString();
            photoAlbum.GroupedBy = groupedBy;
            photoAlbum.DateUpdated = DateTime.Now;

            ctx.PhotoAlbums.Update(photoAlbum);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoAlbum = await ctx.PhotoAlbums
                .FirstOrDefaultAsync(t => t.Id == yearMonDTO.Id);

            var yearStr = yearMonDTO.YearInt.ToString();
            var monStr = yearMonDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            photoAlbum.YearStr = yearStr;
            photoAlbum.MonStr = monStr;
            photoAlbum.YearMon = yearMon;
            photoAlbum.YearMonth = yearMonth;
            photoAlbum.DateUpdated = DateTime.Now;

            ctx.PhotoAlbums.Update(photoAlbum);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
