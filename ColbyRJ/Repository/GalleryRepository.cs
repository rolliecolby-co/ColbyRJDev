namespace ColbyRJ.Repository
{
    public class GalleryRepository : IGalleryRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IFileUpload _fileUpload;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;

        public GalleryRepository(
            IDbContextFactory<ApplicationDbContext> ctxFactory,
            IFileUpload fileUpload,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContext,
            IMapper mapper)
        {
            _ctxFactory = ctxFactory;
            _fileUpload = fileUpload;
            _userManager = userManager;
            _httpContext = httpContext;
            _mapper = mapper;
        }

        public async Task<string> CreateSection(GallerySectionDTO sectionDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var section = new GallerySection
            {
                Section = sectionDTO.Section,
                OrderBy = 99
            };

            ctx.GallerySections.Add(section);
            await ctx.SaveChangesAsync();

            return "id-" + section.Id.ToString();
        }

        public async Task<int> DeleteSection(int sectionId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var section = await ctx.GallerySections.FirstOrDefaultAsync(a => a.Id == sectionId);

            ctx.GallerySections.Remove(section);
            return await ctx.SaveChangesAsync();
        }

        public async Task<GallerySectionDTO> GetSection(int sectionId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var section = await ctx.GallerySections
                .Include(a => a.Photos)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == sectionId);

            var sectionDTO = _mapper.Map<GallerySection, GallerySectionDTO>(section);

            return sectionDTO;
        }

        public async Task<List<GallerySectionDTO>> GetSections()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var sections = await ctx.GallerySections
                .AsNoTracking()
                .OrderBy(a => a.OrderBy).ThenBy(a => a.Section)
                .ToListAsync();

            var sectionsDTO = _mapper.Map<List<GallerySection>, List<GallerySectionDTO>>(sections);

            return sectionsDTO;
        }

        public async Task<string> UpdateSection(GallerySectionDTO sectionDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var section = await ctx.GallerySections
                .FirstOrDefaultAsync(a => a.Id == sectionDTO.Id);

            section.Section = sectionDTO.Section;
            section.OrderBy = sectionDTO.OrderBy;

            ctx.GallerySections.Update(section);
            await ctx.SaveChangesAsync();

            return "ok";
        }


        public async Task<string> CreateDecade(GalleryDecadeDTO decadeDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var decade = new GalleryDecade
            {
                Decade = decadeDTO.Decade
            };

            ctx.GalleryDecades.Add(decade);
            await ctx.SaveChangesAsync();

            return "id-" + decade.Id.ToString();
        }

        public async Task<int> DeleteDecade(int hintId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var decade = await ctx.GalleryDecades.FirstOrDefaultAsync(a => a.Id == hintId);

            ctx.GalleryDecades.Remove(decade);
            return await ctx.SaveChangesAsync();
        }

        public async Task<GalleryDecadeDTO> GetDecade(int decadeId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var decade = await ctx.GalleryDecades
                .Include(a => a.Photos)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == decadeId);

            var decadeDTO = _mapper.Map<GalleryDecade, GalleryDecadeDTO>(decade);

            return decadeDTO;
        }

        public async Task<List<GalleryDecadeDTO>> GetDecades()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var decades = await ctx.GalleryDecades
                .AsNoTracking()
                .OrderBy(a => a.Decade)
                .ToListAsync();

            var decadesDTO = _mapper.Map<List<GalleryDecade>, List<GalleryDecadeDTO>>(decades);

            return decadesDTO;
        }

        public async Task<string> UpdateDecade(GalleryDecadeDTO decadeDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var decade = await ctx.GalleryDecades
                .FirstOrDefaultAsync(a => a.Id == decadeDTO.Id);

            decade.Decade = decadeDTO.Decade;

            ctx.GalleryDecades.Update(decade);
            await ctx.SaveChangesAsync();

            return "ok";
        }


        public async Task<string> CreatePhoto(GalleryPhotoDTO photoDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var yearInt = 0;
            var monthInt = 0;

            if (photoDTO.YearStr != null && photoDTO.YearStr.Length > 0)
            {
                yearInt = Convert.ToInt32(photoDTO.YearStr);
            }

            if (photoDTO.MonthStr != null && photoDTO.MonthStr.Length > 0)
            {
                monthInt = Convert.ToInt32(photoDTO.MonthStr);
            }

            var photo = new GalleryPhoto
            {
                Caption = photoDTO.Caption,
                OrderBy = 99,
                PhotoYearInt = yearInt,
                PhotoMonthInt = monthInt,
                PhotoUrl = string.Empty,
                Owner = appUser.DisplayName,
                GalleryDecadeId = photoDTO.GalleryDecadeId,
                GallerySectionId = photoDTO.GallerySectionId,
                DateUpdated = DateTime.Now
            };

            ctx.GalleryPhotos.Add(photo);
            await ctx.SaveChangesAsync();

            return "id-" + photo.Id.ToString();
        }

        public async Task<int> DeletePhoto(int photoId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photo = await ctx.GalleryPhotos.FirstOrDefaultAsync(a => a.Id == photoId);

            var photoUrl = photo.PhotoUrl;
            var photoName = photoUrl.Replace($"photoGallery/", "");

            var result = _fileUpload.DeleteFile(photoName, "photoGallery");

            ctx.GalleryPhotos.Remove(photo);
            return await ctx.SaveChangesAsync();
        }

        public async Task<GalleryPhotoDTO> GetPhoto(int photoId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photo = await ctx.GalleryPhotos
                .Include(a => a.Decade)
                .Include(a => a.Section)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == photoId);

            var photoDTO = _mapper.Map<GalleryPhoto, GalleryPhotoDTO>(photo);

            if (photoDTO.PhotoYearInt > 0)
            {
                photoDTO.YearStr = photoDTO.PhotoYearInt.ToString();
            }
            if (photoDTO.PhotoMonthInt > 0)
            {
                photoDTO.MonthStr = photoDTO.PhotoMonthInt.ToString();
            }

            return photoDTO;
        }

        public async Task<List<GalleryPhotoDTO>> GetPhotos()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var photos = await ctx.GalleryPhotos
                .Include(a => a.Decade)
                .Include(a => a.Section)
                .Where(q => q.Owner == appUser.DisplayName || appUser.Role == "Admin")
                .AsNoTracking()
                .ToListAsync();

            var photosDTO = _mapper.Map<List<GalleryPhoto>, List<GalleryPhotoDTO>>(photos);

            return photosDTO;
        }

        public async Task<List<GalleryPhotoDTO>> GetAllPhotos()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var photos = await ctx.GalleryPhotos
                .Include(a => a.Decade)
                .Include(a => a.Section)
                .AsNoTracking()
                .ToListAsync();

            var photosDTO = _mapper.Map<List<GalleryPhoto>, List<GalleryPhotoDTO>>(photos);

            return photosDTO;
        }

        public async Task<string> UpdatePhoto(GalleryPhotoDTO photoDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photo = await ctx.GalleryPhotos
                .FirstOrDefaultAsync(a => a.Id == photoDTO.Id);

            photo.Caption = photoDTO.Caption;
            photo.OrderBy = photoDTO.OrderBy;
            photo.PhotoYearInt = photoDTO.PhotoYearInt;
            photo.PhotoMonthInt = photoDTO.PhotoMonthInt;
            photo.PhotoUrl = photoDTO.PhotoUrl;
            photo.GallerySectionId = photoDTO.GallerySectionId;
            photo.GalleryDecadeId = photoDTO.GalleryDecadeId;
            photo.DateUpdated = DateTime.Now;

            ctx.GalleryPhotos.Update(photo);
            await ctx.SaveChangesAsync();

            return "ok";
        }

    }
}
