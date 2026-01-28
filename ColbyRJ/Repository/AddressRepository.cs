namespace ColbyRJ.Repository
{
    public class AddressRepository : IAddressRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IUtilityRepository _utility;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public AddressRepository(
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

        public async Task<string> Create(AddressDTO addressDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var yearStr = addressDTO.YearInt.ToString();
            var monStr = addressDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            var address = new AddressHistory
            {
                Who = addressDTO.Who,
                //StartDate = addressDTO.StartDate,
                YearStr = yearStr,
                MonStr = monStr,
                YearMon = yearMon,
                YearMonth = yearMonth,
                HomeAddress = addressDTO.HomeAddress,
                Remarks = addressDTO.Remarks,
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                DateUpdated = DateTime.Now
            };

            ctx.Addresses.Add(address);
            await ctx.SaveChangesAsync();

            return "id-" + address.Id.ToString();
        }

        public async Task<int> Delete(int addressId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var address = await ctx.Addresses.FirstOrDefaultAsync(a => a.Id == addressId);

            ctx.Addresses.Remove(address);
            return await ctx.SaveChangesAsync();
        }

        public async Task<AddressDTO> GetAddress(int addressId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var address = await ctx.Addresses
                .Include(x => x.Photos.OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate))
                .Include(a => a.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == addressId);

            if (address == null)
            {
                return null;
            }

            var addressDTO = _mapper.Map<AddressHistory, AddressDTO>(address);

            addressDTO.YearInt = Convert.ToInt32(addressDTO.YearStr);
            //addressDTO.StartDateStr = addressDTO.StartDate.ToString("MMM yyyy");

            return addressDTO;
        }

        public async Task<List<AddressDTO>> GetAddresses()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var addresses = await ctx.Addresses
                .Include(x => x.Photos.OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate))
                .Include(a => a.Comments.OrderByDescending(a => a.Id))
                .Where(q => q.OwnerEmail == appUser.Email || appUser.Role == "Admin")
                .AsNoTracking()
                .OrderBy(a => a.Who)
                .ThenBy(a => a.YearMon)
                .ToListAsync();

            if (addresses == null)
            {
                return null;
            }

            var addressesDTO = _mapper.Map<List<AddressHistory>, List<AddressDTO>>(addresses);

            addressesDTO.ForEach(a =>
            {
                a.StartDateStr = a.StartDate.ToString("MMM yyyy");
                if (a.Remarks.Length > 0)
                {
                    a.WithRemarks = "yes";
                }

                if (a.Photos.Count > 0)
                {
                    a.PhotoCount = a.Photos.Count.ToString();
                }

                if (a.Comments.Count > 0)
                {
                    a.CommentCount = a.Comments.Count.ToString();
                }
            });

            return addressesDTO;
        }

        public async Task<List<AddressDTO>> GetBrowseAddresses()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var addresses = await ctx.Addresses
                .Include(x => x.Photos.OrderBy(a => a.OrderBy).ThenBy(b => b.PhotoDate))
                .Include(a => a.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .OrderBy(a => a.Who)
                .ThenBy(a => a.YearMon)
                .ToListAsync();

            if (addresses == null)
            {
                return null;
            }

            var addressesDTO = _mapper.Map<List<AddressHistory>, List<AddressDTO>>(addresses);

            addressesDTO.ForEach(a =>
            {
                a.StartDateStr = a.StartDate.ToString("MMM yyyy");
                if (a.Remarks.Length > 0)
                {
                    a.WithRemarks = "Yes";
                }

                if (a.Photos.Count > 0)
                {
                    a.PhotoCount = a.Photos.Count.ToString();
                }

                if (a.Comments.Count > 0)
                {
                    a.CommentCount = a.Comments.Count.ToString();
                }
            });

            return addressesDTO;
        }

        public async Task<AddressDTO> Update(AddressDTO addressDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var address = await ctx.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressDTO.Id);

            address.Who = addressDTO.Who;
            //address.StartDate = addressDTO.StartDate;
            address.HomeAddress = addressDTO.HomeAddress;
            address.Remarks = addressDTO.Remarks;
            address.DateUpdated = DateTime.Now;

            ctx.Addresses.Update(address);
            await ctx.SaveChangesAsync();

            return _mapper.Map<AddressHistory, AddressDTO>(address);
        }

        public async Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var address = await ctx.Addresses
                .FirstOrDefaultAsync(t => t.Id == yearMonDTO.Id);

            var yearStr = yearMonDTO.YearInt.ToString();
            var monStr = yearMonDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            address.YearStr = yearStr;
            address.MonStr = monStr;
            address.YearMon = yearMon;
            address.YearMonth = yearMonth;
            address.DateUpdated = DateTime.Now;

            ctx.Addresses.Update(address);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
