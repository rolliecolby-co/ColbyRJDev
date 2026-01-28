namespace ColbyRJ.Repository
{
    public class UnitRepository : IUnitRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IUtilityRepository _utility;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public UnitRepository(
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

        public async Task<string> Create(UnitDTO unitDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var yearStr = unitDTO.YearInt.ToString();
            var monStr = unitDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            var unit = new UnitHistory
            {
                //Who = unitDTO.Who,
                Who = appUser.DisplayName,
                //StartDate = unitDTO.StartDate,
                YearStr = yearStr,
                MonStr = monStr,
                YearMon = yearMon,
                YearMonth = yearMonth,
                MilitaryUnit = unitDTO.MilitaryUnit,
                UnitLocation = unitDTO.UnitLocation,
                Remarks = unitDTO.Remarks,
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                DateUpdated = DateTime.Now
            };

            ctx.Units.Add(unit);
            await ctx.SaveChangesAsync();

            return "id-" + unit.Id.ToString();
        }

        public async Task<int> Delete(int unitId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var unit = await ctx.Units.FirstOrDefaultAsync(a => a.Id == unitId);

            ctx.Units.Remove(unit);
            return await ctx.SaveChangesAsync();
        }

        public async Task<List<UnitDTO>> GetBrowseUnits()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var units = await ctx.Units
                .Include(u => u.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .OrderBy(a => a.Who)
                .ThenBy(b => b.YearMon)
                .ToListAsync();

            if (units == null)
            {
                return null;
            }

            var unitsDTO = _mapper.Map<List<UnitHistory>, List<UnitDTO>>(units);

            unitsDTO.ForEach(u =>
            {
                u.StartDateStr = u.StartDate.ToString("MMM yyyy");
                if (u.Remarks.Length > 0)
                {
                    u.WithRemarks = "yes";
                }

                if (u.Comments.Count > 0)
                {
                    u.CommentCount = u.Comments.Count.ToString();
                }
            });

            return unitsDTO;
        }

        public async Task<UnitDTO> GetUnit(int unitId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var unit = await ctx.Units
                .Include(u => u.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == unitId);

            if (unit == null)
            {
                return null;
            }

            var unitDTO = _mapper.Map<UnitHistory, UnitDTO>(unit);


            unitDTO.YearInt = Convert.ToInt32(unitDTO.YearStr);
            //unitDTO.StartDateStr = unitDTO.StartDate.ToString("MMM yyyy");

            return unitDTO;
        }

        public async Task<List<UnitDTO>> GetUnits()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var units = await ctx.Units
                .Include(u => u.Comments.OrderByDescending(a => a.Id))
                .Where(q => q.OwnerEmail == appUser.Email || appUser.Role == "Admin")
                .AsNoTracking()
                .OrderBy(a => a.Who)
                .ThenBy(a => a.YearMon)
                .ToListAsync();

            if (units == null)
            {
                return null;
            }

            var unitsDTO = _mapper.Map<List<UnitHistory>, List<UnitDTO>>(units);

            unitsDTO.ForEach(u =>
            {
                u.StartDateStr = u.StartDate.ToString("MMM yyyy");
                if (u.Remarks.Length > 0)
                {
                    u.WithRemarks = "yes";
                }

                if (u.Comments.Count > 0)
                {
                    u.CommentCount = u.Comments.Count.ToString();
                }
            });

            return unitsDTO;
        }

        public async Task<string> Update(UnitDTO unitDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var unit = await ctx.Units
                .FirstOrDefaultAsync(a => a.Id == unitDTO.Id);

            //unit.Who = unitDTO.Who;
            //unit.StartDate = unitDTO.StartDate;
            unit.MilitaryUnit = unitDTO.MilitaryUnit;
            unit.UnitLocation = unitDTO.UnitLocation;
            unit.Remarks = unitDTO.Remarks;
            unit.DateUpdated = DateTime.Now;

            ctx.Units.Update(unit);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var unit = await ctx.Units
                .FirstOrDefaultAsync(t => t.Id == yearMonDTO.Id);

            var yearStr = yearMonDTO.YearInt.ToString();
            var monStr = yearMonDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            unit.YearStr = yearStr;
            unit.MonStr = monStr;
            unit.YearMon = yearMon;
            unit.YearMonth = yearMonth;
            unit.DateUpdated = DateTime.Now;

            ctx.Units.Update(unit);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
