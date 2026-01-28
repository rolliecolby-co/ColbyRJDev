namespace ColbyRJ.Repository
{
    public class WhoAmIRepository : IWhoAmIRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public WhoAmIRepository(
            IDbContextFactory<ApplicationDbContext> ctxFactory,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IHttpContextAccessor httpContext)
        {
            _ctxFactory = ctxFactory;
            _userManager = userManager;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<string> Create(WhoAmIDTO whoAmIDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var whoAmI = new WhoAmI
            {
                Title = whoAmIDTO.Title,
                Remarks = whoAmIDTO.Remarks,
                OrderBy = whoAmIDTO.OrderBy,
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                DateUpdated = DateTime.Now
            };

            ctx.WhoAmI.Add(whoAmI);
            await ctx.SaveChangesAsync();

            return "id-" + whoAmI.Id.ToString();
        }

        public async Task<int> Delete(int whoAmIId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var whoAmI = await ctx.WhoAmI.FirstOrDefaultAsync(t => t.Id == whoAmIId);

            ctx.WhoAmI.Remove(whoAmI);
            return await ctx.SaveChangesAsync();
        }

        public async Task<List<WhoAmIDTO>> GetWhoAmIs()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var whoAmIs = await ctx.WhoAmI
                .Where(q => q.OwnerEmail == appUser.Email || appUser.Role == "Admin")
                .AsNoTracking()
                .OrderBy(w => w.Owner).ThenBy(w => w.OrderBy).ThenBy(w => w.Title)
                .ToListAsync();

            var whoAmIsDTO = _mapper.Map<List<WhoAmI>, List<WhoAmIDTO>>(whoAmIs);

            whoAmIsDTO.ForEach(w =>
            {
                if (w.AudioFilename.Length > 0)
                {
                    w.WithAudio = "Audio";
                }
            });

            return whoAmIsDTO;
        }

        public async Task<List<WhoAmIDTO>> GetAllWhoAmIs()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var whoAmIs = await ctx.WhoAmI
                .AsNoTracking()
                .OrderBy(w => w.Owner).ThenBy(w => w.OrderBy).ThenBy(w => w.Title)
                .ToListAsync();

            var whoAmIsDTO = _mapper.Map<List<WhoAmI>, List<WhoAmIDTO>>(whoAmIs);

            whoAmIsDTO.ForEach(w =>
            {
                if (w.AudioFilename.Length > 0)
                {
                    w.WithAudio = "Audio";
                }
            });

            return whoAmIsDTO;
        }

        public async Task<List<WhoAmIDTO>> GetBrowseWhoAmIs()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var whoAmIs = await ctx.WhoAmI
                .AsNoTracking()
                .OrderBy(w => w.Owner).ThenBy(w => w.OrderBy).ThenBy(w => w.Title)
                .ToListAsync();

            var whoAmIsDTO = _mapper.Map<List<WhoAmI>, List<WhoAmIDTO>>(whoAmIs);

            whoAmIsDTO.ForEach(w =>
            {
                if (w.AudioFilename.Length > 0)
                {
                    w.WithAudio = "Yes";
                }
            });

            return whoAmIsDTO;
        }

        public async Task<WhoAmIDTO> GetWhoAmI(int whoAmIId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var whoAmI = await ctx.WhoAmI
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == whoAmIId);

            var whoAmIDTO = _mapper.Map<WhoAmI, WhoAmIDTO>(whoAmI);

            return whoAmIDTO;
        }

        public async Task<string> Update(WhoAmIDTO whoAmIDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var whoAmI = await ctx.WhoAmI
                .FirstOrDefaultAsync(t => t.Id == whoAmIDTO.Id);

            whoAmI.Title = whoAmIDTO.Title;
            whoAmI.Remarks = whoAmIDTO.Remarks;
            whoAmI.OrderBy = whoAmIDTO.OrderBy;
            whoAmI.AudioFilename = whoAmIDTO.AudioFilename;
            whoAmI.AudioUrl = whoAmIDTO.AudioUrl;
            whoAmI.AudioNote = whoAmIDTO.AudioNote;
            whoAmI.DateUpdated = DateTime.Now;

            ctx.WhoAmI.Update(whoAmI);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<WhoAmIDTO> GetWhoAmIByOwner(string owner)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var whoAmI = await ctx.WhoAmI
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Owner == owner);

            var whoAmIDTO = _mapper.Map<WhoAmI, WhoAmIDTO>(whoAmI);

            return whoAmIDTO;
        }
    }
}
