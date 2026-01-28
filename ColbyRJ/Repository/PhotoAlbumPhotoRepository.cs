namespace ColbyRJ.Repository
{
    public class PhotoAlbumPhotoRepository : IPhotoAlbumPhotoRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IFileUpload _fileUpload;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public PhotoAlbumPhotoRepository(
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

        public async Task<string> Create(PhotoAlbumPhotoDTO photoDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var photo = new PhotoAlbumPhoto
            {
                Caption = photoDTO.Caption,
                PhotoAlbumId = photoDTO.PhotoAlbumId,
                PhotoDate = photoDTO.PhotoDate,
                PhotoUrl = photoDTO.PhotoUrl,
                OrderBy = 99,
                DateUpdated = DateTime.Now
            };

            await ctx.PhotoAlbumPhotos.AddAsync(photo);
            await ctx.SaveChangesAsync();

            return "id-" + photo.Id.ToString();
        }

        public async Task<int> Delete(int photoId)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var photo = await ctx.PhotoAlbumPhotos.FirstOrDefaultAsync(x => x.Id == photoId);


            var photoUrl = photo.PhotoUrl;
            var photoName = photoUrl.Replace($"PhotoAlbumPhotos/", "");

            var result = _fileUpload.DeleteFile(photoName, "PhotoAlbumPhotos");

            ctx.PhotoAlbumPhotos.Remove(photo);
            return await ctx.SaveChangesAsync();
        }

        public async Task<int> DeletePhotoByPhotoUrl(string photoUrl)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var allPhotos = await ctx.PhotoAlbumPhotos.FirstOrDefaultAsync
                                (x => x.PhotoUrl.ToLower() == photoUrl.ToLower());
            if (allPhotos == null)
            {
                return 0;
            }
            ctx.PhotoAlbumPhotos.Remove(allPhotos);
            return await ctx.SaveChangesAsync();
        }

        public async Task<PhotoAlbumPhotoDTO> GetPhoto(int photoId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photo = await ctx.PhotoAlbumPhotos
                .Include(q => q.PhotoAlbum)
                .AsNoTracking()
               .FirstOrDefaultAsync(q => q.Id == photoId);

            var photoDTO = _mapper.Map<PhotoAlbumPhoto, PhotoAlbumPhotoDTO>(photo);

            try
            {
                DateTime photoDate = (DateTime)photoDTO.PhotoDate;
                photoDTO.PhotoDateStr = photoDate.ToString("M/d/yyyy");
            }
            catch { }

            return photoDTO;
        }

        public async Task<List<PhotoAlbumPhotoDTO>> GetPhotos(int photoAlbumId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photos = await ctx.PhotoAlbumPhotos
                .Include(q => q.PhotoAlbum)
                .Where(q => q.PhotoAlbumId == photoAlbumId)
              .OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate)
              .AsNoTracking().ToListAsync();

            var photosDTO = _mapper.Map<List<PhotoAlbumPhoto>, List<PhotoAlbumPhotoDTO>>(photos);

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

        public async Task<string> Update(PhotoAlbumPhotoDTO photoDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photo = await ctx.PhotoAlbumPhotos.FirstOrDefaultAsync(q => q.Id == photoDTO.Id);

            photo.Caption = photoDTO.Caption;
            photo.OrderBy = photoDTO.OrderBy;
            photo.PhotoDate = photoDTO.PhotoDate;
            photo.DateUpdated = DateTime.Now;

            ctx.PhotoAlbumPhotos.Update(photo);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
