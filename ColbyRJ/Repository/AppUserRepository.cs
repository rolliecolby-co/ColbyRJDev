using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;

namespace ColbyRJ.Repository
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;

        public AppUserRepository(
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            IEmailSender emailSender,
            IMapper mapper,
            IHttpContextAccessor httpContext,
            IDbContextFactory<ApplicationDbContext> ctxFactory)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _emailSender = emailSender;
            _mapper = mapper;
            _httpContext = httpContext;
            _ctxFactory = ctxFactory;
        }

        public async Task<string> Create(AppUserCreateDTO appUserDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var emailCheck = ctx.AppUsers
                .Where(q => q.Email == appUserDTO.Email);
            if (emailCheck.Any())
            {
                return "This Email is already registered";
            }

            var appUser = new AppUser
            {
                Name = appUserDTO.Name,
                DisplayName = appUserDTO.DisplayName,
                Email = appUserDTO.Email,
                Role = "User",
                InfoOptIn = true
            };

            ctx.AppUsers.Add(appUser);
            await ctx.SaveChangesAsync();

            var user = new ApplicationUser
            {

                UserName = appUser.Email,
                Email = appUser.Email
            };

            var result = await _userManager.CreateAsync(user, "Init!2345");

            await _userManager.AddToRoleAsync(user, "User");

            if (result.Succeeded)
            {
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string appUrl = "";
                var path = _webHostEnvironment.ContentRootPath;

                if (path.ToLower().Contains("colbyrjdev"))
                {
                    appUrl = _configuration["LocalUrl"];
                }
                else
                {
                    appUrl = _configuration["AppUrl"];
                }

                var url = $"{appUrl}/account/confirmemail?userid={user.Id}&code={validEmailToken}";

                await _emailSender.SendEmailAsync(appUser.Email, "Confirm your email",
                    $"<p>Your ColbyRJ.us Family Memories website user is registered. " +
                    $"Please <a href='{url}'>click here</a> to confirm your email address.</p>" +

                    $"<p>After you confirm your email address and reset your password, you will be able to log in to the website." +

                    $"<p>To reset your password, use the link on the ColbyRJ.us Login page.</p>"
                    );
            }

            return "id-" + appUser.Id.ToString();
        }

        public async Task<int> Delete(int appUserId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await ctx.AppUsers.FirstOrDefaultAsync(t => t.Id == appUserId);

            ctx.AppUsers.Remove(user);
            return await ctx.SaveChangesAsync();
        }

        public async Task<List<LoginTallyDTO>> GetLoginTally()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var logins = await ctx.LoginRecord
                .Where(q => q.LoginDate > DateTime.Now.AddDays(-90))
                .OrderBy(q => q.Id)
                .ToListAsync();

            List<LoginTallyDTO> loginTallyList = new List<LoginTallyDTO>();
            LoginTallyDTO loginTally = new LoginTallyDTO();

            logins.ForEach(l =>
            {
                var prevItem = loginTallyList.Where(q => q.DisplayName == l.DisplayName).ToList();
                if (!prevItem.Any())
                {
                    loginTally = new LoginTallyDTO();
                    loginTally.DisplayName = l.DisplayName;
                    loginTally.Email = l.Email;
                    loginTally.NumberOfLoginsLast90Days = 1;
                    loginTally.LastLoginDate = l.LoginDate;
                    loginTallyList.Add(loginTally);
                }
            });

            loginTallyList.ForEach(t =>
            {
                t.NumberOfLoginsLast90Days = logins
                                                .Where(q => q.DisplayName == t.DisplayName && q.Email == t.Email)
                                                .Count();

                t.LastLoginDate = logins
                                     .Where(q => q.DisplayName == t.DisplayName && q.Email == t.Email)
                                     .OrderByDescending(q => q.LoginDate)
                                     .Select(q => q.LoginDate)
                                     .FirstOrDefault();

                t.LastLoginDateStr = t.LastLoginDate.ToString("M/d/yyyy");
            });

            loginTallyList = loginTallyList.OrderBy(a => a.DisplayName).ToList();

            return loginTallyList;
        }

        public async Task<List<OwnerDTO>> GetOwners()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var users = await ctx.AppUsers
                .AsNoTracking()
            .ToListAsync();

            List<OwnerDTO> ownerDTO = new List<OwnerDTO>();
            OwnerDTO owner = new OwnerDTO();

            users.ForEach(u =>
            {
                owner = new OwnerDTO();
                owner.Owner = u.DisplayName;
                owner.OwnerEmail = u.Email;
                ownerDTO.Add(owner);
            });

            ownerDTO = ownerDTO.OrderBy(o => o.Owner).ToList();

            return ownerDTO;
        }

        public async Task<AppUserDTO> GetUser(int appUserId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var appUser = await ctx.AppUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == appUserId);

            IdentityUser user = await _userManager.FindByEmailAsync(appUser.Email);

            var userDTO = _mapper.Map<AppUser, AppUserDTO>(appUser);

            if (!user.EmailConfirmed)
            {
                userDTO.EmailConfirmed = "Email Not Confirmed";
            }

            return userDTO;
        }

        public async Task<AppUserDTO> GetUserByEmail(string email)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await ctx.AppUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            var userDTO = _mapper.Map<AppUser, AppUserDTO>(user);

            return userDTO;
        }

        public async Task<List<AppUserDTO>> GetUsers()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var users = await ctx.AppUsers
                .AsNoTracking()
                .OrderBy(u => u.DisplayName).ThenBy(u => u.Name)
            .ToListAsync();

            var usersDTO = _mapper.Map<List<AppUser>, List<AppUserDTO>>(users);

            return usersDTO;
        }

        public async Task<List<WhoDTO>> GetWho(string indAll)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);

            var users = await ctx.AppUsers
                .Where(q => q.Email == user.Email)
                .AsNoTracking()
            .ToListAsync();

            var couples = await ctx.AppUsers
                .Where(q => q.DisplayCouple.Length > 1 && q.Email == user.Email)
                .AsNoTracking()
            .ToListAsync();

            List<WhoDTO> whoDTO = new List<WhoDTO>();
            WhoDTO who = new WhoDTO();

            users.ForEach(u =>
            {
                who = new WhoDTO();
                who.Who = u.DisplayName;
                whoDTO.Add(who);
            });

            if (indAll == "all")
            {
                couples.ForEach(u =>
                {
                    var prevItem = whoDTO.Where(q => q.Who == u.DisplayCouple).ToList();
                    if (!prevItem.Any())
                    {
                        who = new WhoDTO();
                        who.Who = u.DisplayCouple;
                        whoDTO.Add(who);
                    }
                });
            }

            whoDTO = whoDTO.OrderBy(w => w.Who).ToList();

            return whoDTO;
        }

        public async Task<string> Update(AppUserDTO appUserDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            bool value = await ctx.AppUsers
                .Where(q => q.Email != appUserDTO.Email)
                .AnyAsync(q => q.DisplayName.ToLower().Trim() == appUserDTO.DisplayName.ToLower().Trim());
            if (value)
            {
                return $"Display Name {appUserDTO.DisplayName} is not available.";
            }

            var appUser = await ctx.AppUsers
                .FirstOrDefaultAsync(u => u.Id == appUserDTO.Id);

            appUser.Name = appUserDTO.Name;
            appUser.DisplayName = appUserDTO.DisplayName;
            appUser.DisplayCouple = appUserDTO.DisplayCouple;
            appUser.Address = appUserDTO.Address;
            appUser.Hobbies = appUserDTO.Hobbies;
            appUser.Interests = appUserDTO.Interests;
            appUser.Role = appUserDTO.Role;
            appUser.Note = appUserDTO.Note;
            appUser.MobilePhone = appUserDTO.MobilePhone;
            appUser.HomePhone = appUserDTO.HomePhone;
            appUser.InfoOptIn = appUserDTO.InfoOptIn;
            appUser.DOB = appUserDTO.DOB;
            appUser.WeddingDate = appUserDTO.WeddingDate;

            ctx.AppUsers.Update(appUser);
            await ctx.SaveChangesAsync();

            var previousRole = appUserDTO.PreviousRole;
            var currentRole = appUser.Role;

            ApplicationUser user = await _userManager.FindByEmailAsync(appUser.Email);

            if (previousRole != currentRole)
            {
                if (!string.IsNullOrEmpty(previousRole))
                {
                    await _userManager.RemoveFromRoleAsync(user, previousRole);
                }
                await _userManager.AddToRoleAsync(user, currentRole);
            }

            return "ok";
        }
    }
}
