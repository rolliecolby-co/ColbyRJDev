namespace ColbyRJ.Repository
{
    public class PhotoFolderRepository : IPhotoFolderRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IUtilityRepository _utility;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly string rootPath = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\photoDirs\"}";
        private readonly string rootFolder = "/photoDirs/";

        public PhotoFolderRepository(
            IDbContextFactory<ApplicationDbContext> ctxFactory,
            IMapper mapper,
            IUtilityRepository utility,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContext)
        {
            _ctxFactory = ctxFactory;
            _mapper = mapper;
            _utility = utility;
            _userManager = userManager;
            _httpContext = httpContext;
        }

        public async Task<string> Create(PhotoFolderCreateDTO photoFolderDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var category = "New";
            var groupedBy = "New";

            var yearStr = photoFolderDTO.YearInt.ToString();
            var monStr = photoFolderDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            var photoFolder = new PhotoFolder
            {
                Title = photoFolderDTO.Title,

                Category = "New",
                GroupedBy = "New",
                YearStr = yearStr,
                MonStr = monStr,
                YearMon = yearMon,
                YearMonth = yearMonth,
                Element = "Photo Folder",
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                DateUpdated = DateTime.Now
            };

            ctx.PhotoFolders.Add(photoFolder);
            await ctx.SaveChangesAsync();

            Random rnd = new Random();
            var key = photoFolder.Id.ToString() + "-" + rnd.Next(photoFolder.Id * 7, photoFolder.Id * 123).ToString();
            photoFolder.Key = key;

            ctx.PhotoFolders.Update(photoFolder);
            await ctx.SaveChangesAsync();

            return "id-" + photoFolder.Id.ToString();
        }

        public async Task<int> Delete(int photoFolderId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoFolder = await ctx.PhotoFolders.FirstOrDefaultAsync(t => t.Id == photoFolderId);

            ctx.PhotoFolders.Remove(photoFolder);
            return await ctx.SaveChangesAsync();
        }

        public async Task<List<PhotoFolderDTO>> GetActivePhotoFolders()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoFolders = await ctx.PhotoFolders
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .Where(q => q.Active == true)
                .AsNoTracking()
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.YearMon).ThenBy(s => s.Title)
            .ToListAsync();

            var photoFoldersDTO = _mapper.Map<List<PhotoFolder>, List<PhotoFolderDTO>>(photoFolders);

            photoFoldersDTO.ForEach(f =>
            {
                var photoCount = 0;
                if (f.PathFolder.Length > 0)
                {
                    var pathFolder = f.PathFolder;
                    var pathDir = pathFolder.Replace("/", "\\");
                    var path = rootPath + pathDir;
                    var files = Directory.GetFiles(path);
                    foreach (var file in files)
                    {
                        photoCount++;
                    }
                    f.PhotoCount = photoCount.ToString();
                }

                if (f.Comments.Count > 0)
                {
                    f.CommentCount = f.Comments.Count.ToString();
                }

                f.Decade = f.YearStr.Substring(0, 3) + "0s";

            });

            return photoFoldersDTO;
        }

        public async Task<PhotoFolderDTO> GetPhotoFolder(int photoFolderId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoFolder = await ctx.PhotoFolders
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == photoFolderId);

            if (photoFolder == null)
            {
                return null;
            }

            var photoFolderDTO = _mapper.Map<PhotoFolder, PhotoFolderDTO>(photoFolder);

            photoFolderDTO.YearInt = Convert.ToInt32(photoFolderDTO.YearStr);
            photoFolderDTO.Decade = photoFolderDTO.YearStr.Substring(0, 3) + "0s";

            var photoCount = 0;
            if (photoFolderDTO.PathFolder.Length > 0)
            {
                var pathFolder = photoFolderDTO.PathFolder;
                var pathDir = pathFolder.Replace("/", "\\");
                var path = rootPath + pathDir;
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    photoCount++;
                }
                photoFolderDTO.PhotoCount = photoCount.ToString() + " Photo(s)";
            }

            return photoFolderDTO;
        }

        public async Task<PhotoFolderDTO> GetPhotoFolderByKey(string key)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoFolder = await ctx.PhotoFolders
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Key == key);

            if (photoFolder == null)
            {
                return null;
            }

            var photoFolderDTO = _mapper.Map<PhotoFolder, PhotoFolderDTO>(photoFolder);

            photoFolderDTO.YearInt = Convert.ToInt32(photoFolderDTO.YearStr);
            photoFolderDTO.Decade = photoFolderDTO.YearStr.Substring(0, 3) + "0s";

            var photoCount = 0;
            if (photoFolderDTO.PathFolder.Length > 0)
            {
                var pathFolder = photoFolderDTO.PathFolder;
                var pathDir = pathFolder.Replace("/", "\\");
                var path = rootPath + pathDir;
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    photoCount++;
                }
                photoFolderDTO.PhotoCount = photoCount.ToString() + " Photo(s)";
            }

            return photoFolderDTO;
        }

        public async Task<List<PhotoFolderDTO>> GetPhotoFolders()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var photoFolders = await ctx.PhotoFolders
                .Include(s => s.Comments.OrderByDescending(a => a.Id))
                .Where(q => q.OwnerEmail == appUser.Email || appUser.Role == "Admin")
                .AsNoTracking()
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.Title)
            .ToListAsync();

            var photoFoldersDTO = _mapper.Map<List<PhotoFolder>, List<PhotoFolderDTO>>(photoFolders);

            photoFoldersDTO.ForEach(f =>
            {
                var photoCount = 0;
                if (f.PathFolder.Length > 0)
                {
                    var pathFolder = f.PathFolder;
                    var pathDir = pathFolder.Replace("/", "\\");
                    var path = rootPath + pathDir;
                    var files = Directory.GetFiles(path);
                    foreach (var file in files)
                    {
                        photoCount++;
                    }
                    f.PhotoCount = photoCount.ToString() + " Photo(s)";
                }
                if (f.Active)
                {
                    f.ActiveStr = "yes";
                }

                if (f.Comments.Count > 0)
                {
                    f.CommentCount = f.Comments.Count.ToString();
                }

                f.Decade = f.YearStr.Substring(0, 3) + "0s";

            });

            return photoFoldersDTO;
        }

        public async Task<string> Update(PhotoFolderDTO photoFolderDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoFolder = await ctx.PhotoFolders
                .FirstOrDefaultAsync(t => t.Id == photoFolderDTO.Id);

            photoFolder.Title = photoFolderDTO.Title;

            photoFolder.Owner = photoFolderDTO.Owner;
            photoFolder.OwnerEmail = photoFolderDTO.OwnerEmail;
            photoFolder.Active = photoFolderDTO.Active;
            photoFolder.PathFolder = photoFolderDTO.PathFolder;
            photoFolder.Remarks = photoFolderDTO.Remarks;
            photoFolder.ShowFilename = photoFolderDTO.ShowFilename;
            photoFolder.DateUpdated = DateTime.Now;

            var updatedPhotoFolder = ctx.PhotoFolders.Update(photoFolder);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<string> UpdateGroupBy(GroupByEditDTO groupByDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoFolder = await ctx.PhotoFolders
                .FirstOrDefaultAsync(t => t.Id == groupByDTO.Id);

            var category = groupByDTO.Category;
            var section = groupByDTO.Section.ToString();
            var topic = groupByDTO.Topic.ToString();
            var groupedBy = await _utility.GetGroupedBy(category, section, topic);

            photoFolder.Category = groupByDTO.Category;
            photoFolder.Section = groupByDTO.Section.ToString();
            photoFolder.Topic = groupByDTO.Topic.ToString();
            photoFolder.GroupedBy = groupedBy;
            photoFolder.DateUpdated = DateTime.Now;

            ctx.PhotoFolders.Update(photoFolder);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<string> UpdateOwner(OwnerEditDTO ownerDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoFolder = await ctx.PhotoFolders
                .FirstOrDefaultAsync(t => t.Id == ownerDTO.Id);

            photoFolder.Owner = ownerDTO.Owner;
            photoFolder.OwnerEmail = ownerDTO.OwnerEmail;
            photoFolder.DateUpdated = DateTime.Now;

            ctx.PhotoFolders.Update(photoFolder);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoFolder = await ctx.PhotoFolders
                .FirstOrDefaultAsync(t => t.Id == yearMonDTO.Id);

            var yearStr = yearMonDTO.YearInt.ToString();
            var monStr = yearMonDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            photoFolder.YearStr = yearStr;
            photoFolder.MonStr = monStr;
            photoFolder.YearMon = yearMon;
            photoFolder.YearMonth = yearMonth;
            photoFolder.DateUpdated = DateTime.Now;

            ctx.PhotoFolders.Update(photoFolder);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
