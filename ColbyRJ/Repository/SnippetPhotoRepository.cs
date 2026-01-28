namespace ColbyRJ.Repository
{
    public class SnippetPhotoRepository : ISnippetPhotoRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IFileUpload _fileUpload;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public SnippetPhotoRepository(
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

        public async Task<string> Create(SnippetPhotoDTO photoDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var photo = new SnippetPhoto
            {
                Caption = photoDTO.Caption,
                SnippetId = photoDTO.SnippetId,
                PhotoDate = photoDTO.PhotoDate,
                PhotoUrl = photoDTO.PhotoUrl,
                OrderBy = 99,
                DateUpdated = DateTime.Now
            };

            await ctx.SnippetPhotos.AddAsync(photo);
            await ctx.SaveChangesAsync();

            return "id-" + photo.Id.ToString();
        }

        public async Task<int> Delete(int photoId)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var photo = await ctx.SnippetPhotos.FirstOrDefaultAsync(x => x.Id == photoId);


            var photoUrl = photo.PhotoUrl;
            var photoName = photoUrl.Replace($"SnippetPhotos/", "");

            var result = _fileUpload.DeleteFile(photoName, "SnippetPhotos");

            ctx.SnippetPhotos.Remove(photo);
            return await ctx.SaveChangesAsync();
        }

        public async Task<int> DeletePhotoByPhotoUrl(string photoUrl)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var allPhotos = await ctx.SnippetPhotos.FirstOrDefaultAsync
                                (x => x.PhotoUrl.ToLower() == photoUrl.ToLower());
            if (allPhotos == null)
            {
                return 0;
            }
            ctx.SnippetPhotos.Remove(allPhotos);
            return await ctx.SaveChangesAsync();
        }

        public async Task<SnippetPhotoDTO> GetPhoto(int photoId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photo = await ctx.SnippetPhotos
                .Include(q => q.Snippet)
                .AsNoTracking()
               .FirstOrDefaultAsync(q => q.Id == photoId);

            var photoDTO = _mapper.Map<SnippetPhoto, SnippetPhotoDTO>(photo);

            try
            {
                DateTime photoDate = (DateTime)photoDTO.PhotoDate;
                photoDTO.PhotoDateStr = photoDate.ToString("M/d/yyyy");
            }
            catch { }

            return photoDTO;
        }

        public async Task<List<SnippetPhotoDTO>> GetPhotos(int snippetId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photos = await ctx.SnippetPhotos
                .Include(q => q.Snippet)
                .Where(q => q.SnippetId == snippetId)
              .OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate)
              .AsNoTracking().ToListAsync();

            var photosDTO = _mapper.Map<List<SnippetPhoto>, List<SnippetPhotoDTO>>(photos);

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

        public async Task<string> Update(SnippetPhotoDTO photoDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photo = await ctx.SnippetPhotos.FirstOrDefaultAsync(q => q.Id == photoDTO.Id);

            photo.Caption = photoDTO.Caption;
            photo.OrderBy = photoDTO.OrderBy;
            photo.PhotoDate = photoDTO.PhotoDate;
            photo.DateUpdated = DateTime.Now;

            ctx.SnippetPhotos.Update(photo);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
