namespace ColbyRJ.Repository
{
    public class AddressPhotoRepository : IAddressPhotoRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IFileUpload _fileUpload;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public AddressPhotoRepository(
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

        public async Task<string> Create(AddressPhotoDTO photoDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var photo = new AddressPhoto
            {
                Caption = photoDTO.Caption,
                AddressHistoryId = photoDTO.AddressHistoryId,
                PhotoDate = photoDTO.PhotoDate,
                PhotoUrl = photoDTO.PhotoUrl,
                OrderBy = 99,
                DateUpdated = DateTime.Now
            };

            await ctx.AddressPhotos.AddAsync(photo);
            await ctx.SaveChangesAsync();

            return "id-" + photo.Id.ToString();
        }

        public async Task<int> Delete(int photoId)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var photo = await ctx.AddressPhotos.FirstOrDefaultAsync(x => x.Id == photoId);


            var photoUrl = photo.PhotoUrl;
            var photoName = photoUrl.Replace($"AddressPhotos/", "");

            var result = _fileUpload.DeleteFile(photoName, "AddressPhotos");

            ctx.AddressPhotos.Remove(photo);
            return await ctx.SaveChangesAsync();
        }

        public async Task<int> DeletePhotoByPhotoUrl(string photoUrl)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var allPhotos = await ctx.AddressPhotos.FirstOrDefaultAsync
                                (x => x.PhotoUrl.ToLower() == photoUrl.ToLower());
            if (allPhotos == null)
            {
                return 0;
            }
            ctx.AddressPhotos.Remove(allPhotos);
            return await ctx.SaveChangesAsync();
        }

        public async Task<AddressPhotoDTO> GetPhoto(int photoId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photo = await ctx.AddressPhotos
                .Include(q => q.Address)
                .AsNoTracking()
               .FirstOrDefaultAsync(q => q.Id == photoId);

            var photoDTO = _mapper.Map<AddressPhoto, AddressPhotoDTO>(photo);

            try
            {
                DateTime photoDate = (DateTime)photoDTO.PhotoDate;
                photoDTO.PhotoDateStr = photoDate.ToString("M/d/yyyy");
            }
            catch { }

            return photoDTO;
        }

        public async Task<List<AddressPhotoDTO>> GetPhotos(int addressId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photos = await ctx.AddressPhotos
                .Include(q => q.Address)
                .Where(q => q.AddressHistoryId == addressId)
              .OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate)
              .AsNoTracking().ToListAsync();

            var photosDTO = _mapper.Map<List<AddressPhoto>, List<AddressPhotoDTO>>(photos);

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

        public async Task<string> Update(AddressPhotoDTO photoDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photo = await ctx.AddressPhotos.FirstOrDefaultAsync(q => q.Id == photoDTO.Id);

            photo.Caption = photoDTO.Caption;
            photo.OrderBy = photoDTO.OrderBy;
            photo.PhotoDate = photoDTO.PhotoDate;
            photo.DateUpdated = DateTime.Now;

            ctx.AddressPhotos.Update(photo);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
