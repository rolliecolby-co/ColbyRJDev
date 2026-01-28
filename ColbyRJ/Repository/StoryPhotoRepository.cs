namespace ColbyRJ.Repository
{
    public class StoryPhotoRepository : IStoryPhotoRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IFileUpload _fileUpload;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public StoryPhotoRepository(
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

        public async Task<string> Create(StoryPhotoDTO photoDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var photo = new StoryPhoto
            {
                Caption = photoDTO.Caption,
                StoryId = photoDTO.StoryId,
                PhotoDate = photoDTO.PhotoDate,
                PhotoUrl = photoDTO.PhotoUrl,
                OrderBy = 99,
                DateUpdated = DateTime.Now
            };

            await ctx.StoryPhotos.AddAsync(photo);
            await ctx.SaveChangesAsync();

            return "id-" + photo.Id.ToString();
        }

        public async Task<int> Delete(int photoId)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var photo = await ctx.StoryPhotos.FirstOrDefaultAsync(x => x.Id == photoId);


            var photoUrl = photo.PhotoUrl;
            var photoName = photoUrl.Replace($"StoryPhotos/", "");

            var result = _fileUpload.DeleteFile(photoName, "StoryPhotos");

            ctx.StoryPhotos.Remove(photo);
            return await ctx.SaveChangesAsync();
        }

        public async Task<int> DeletePhotoByPhotoUrl(string photoUrl)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var allPhotos = await ctx.StoryPhotos.FirstOrDefaultAsync
                                (x => x.PhotoUrl.ToLower() == photoUrl.ToLower());
            if (allPhotos == null)
            {
                return 0;
            }
            ctx.StoryPhotos.Remove(allPhotos);
            return await ctx.SaveChangesAsync();
        }

        public async Task<StoryPhotoDTO> GetPhoto(int photoId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photo = await ctx.StoryPhotos
                .Include(q => q.Story)
                .AsNoTracking()
               .FirstOrDefaultAsync(q => q.Id == photoId);

            var photoDTO = _mapper.Map<StoryPhoto, StoryPhotoDTO>(photo);

            try
            {
                DateTime photoDate = (DateTime)photoDTO.PhotoDate;
                photoDTO.PhotoDateStr = photoDate.ToString("M/d/yyyy");
            }
            catch { }

            return photoDTO;
        }

        public async Task<List<StoryPhotoDTO>> GetPhotos(int storyId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photos = await ctx.StoryPhotos
                .Include(q => q.Story)
                .Where(q => q.StoryId == storyId)
              .OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate)
              .AsNoTracking().ToListAsync();

            var photosDTO = _mapper.Map<List<StoryPhoto>, List<StoryPhotoDTO>>(photos);

            photosDTO.ForEach(p =>
            {
                try
                {
                    DateTime photoDate = (DateTime)p.PhotoDate;
                    p.PhotoDateStr = photoDate.ToString("M/d/yyyy");
                }
                catch { }
            });

            return photosDTO;
        }

        public async Task<string> Update(StoryPhotoDTO photoDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photo = await ctx.StoryPhotos.FirstOrDefaultAsync(q => q.Id == photoDTO.Id);

            photo.Caption = photoDTO.Caption;
            photo.OrderBy = photoDTO.OrderBy;
            photo.PhotoDate = photoDTO.PhotoDate;
            photo.DateUpdated = DateTime.Now;

            ctx.StoryPhotos.Update(photo);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
