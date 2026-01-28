using MimeKit;

namespace ColbyRJ.Repository
{
    public class BroadcastEmailRepository : IBroadcastEmailRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private bool sendNow = false;

        public BroadcastEmailRepository(
            IDbContextFactory<ApplicationDbContext> ctxFactory,
            IMapper mapper,
            IHttpContextAccessor httpContext,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _ctxFactory = ctxFactory;
            _mapper = mapper;
            _httpContext = httpContext;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<string> Create(BroadcastEmailDTO broadcastEmailDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);
            var users = await ctx.AppUsers.ToListAsync();

            var beSubject = broadcastEmailDTO.Subject;
            var beMsg = broadcastEmailDTO.Message;

            List<string> Emails = new List<string>();
            List<string> SendTos = new List<string>();

            List<string> IDs = new List<string>();
            var ids = broadcastEmailDTO.UserIDs.ToList();

            var sendToAll = "";

            foreach (var item in ids)
            {
                if (item == "All")
                {
                    sendToAll = "yes";
                }
            }

            var sentTo = "";
            if (sendToAll == "yes")
            {
                foreach (var item in users)
                {
                    Emails.Add(item.Email);
                    if (sentTo.Length == 0)
                    {
                        sentTo = item.DisplayName;
                    }
                    else
                    {
                        sentTo = sentTo + ", " + item.DisplayName;
                    }

                    SendBE(item.Email, item.DisplayName, beSubject, beMsg);
                }
            }
            else
            {
                foreach (var item in users)
                {
                    foreach (var item2 in ids)
                    {
                        try
                        {
                            if (item.Id == Convert.ToInt32(item2))
                            {
                                Emails.Add(item.Email);
                                if (sentTo.Length == 0)
                                {
                                    sentTo = item.DisplayName;
                                }
                                else
                                {
                                    sentTo = sentTo + ", " + item.DisplayName;
                                }

                                SendBE(item.Email, item.DisplayName, beSubject, beMsg);
                            }
                        }
                        catch { }
                    }
                }
            }

            var workOptIn = await ctx.WorkOptIn.FirstOrDefaultAsync();
            workOptIn.RefreshDate = DateTime.Now;

            var broadcastEmail = new BroadcastEmail
            {
                SentTo = sentTo,
                Subject = broadcastEmailDTO.Subject,
                Message = broadcastEmailDTO.Message,
                SentBy = appUser.DisplayName,
                DateSent = DateTime.Now
            };

            ctx.BroadcastEmails.Add(broadcastEmail);
            await ctx.SaveChangesAsync();

            return "id-" + broadcastEmail.Id.ToString();
        }

        public async void SendBE(string sendToEmail, string sendToName, string beSubject, string beMsg)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var mailMessage = new MimeMessage();

            mailMessage.Subject = beSubject;

            mailMessage.To.Add(new MailboxAddress(sendToName, sendToEmail));

            var builder = new BodyBuilder();
            builder.HtmlBody = string.Format(beMsg);
            mailMessage.Body = builder.ToMessageBody();

            await _emailService.Send(mailMessage);
        }

        public async Task<int> Delete(int broadcastEmailId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var broadcastEmail = await ctx.BroadcastEmails.FirstOrDefaultAsync(t => t.Id == broadcastEmailId);

            ctx.BroadcastEmails.Remove(broadcastEmail);
            return await ctx.SaveChangesAsync();
        }

        public async Task<BroadcastEmailDTO> GetBroadcastEmail(int broadcastEmailId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var broadcastEmail = await ctx.BroadcastEmails
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == broadcastEmailId);

            if (broadcastEmail == null)
            {
                return null;
            }

            var broadcastEmailDTO = _mapper.Map<BroadcastEmail, BroadcastEmailDTO>(broadcastEmail);

            return broadcastEmailDTO;
        }

        public async Task<List<BroadcastEmailDTO>> GetBroadcastEmails()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var broadcastEmails = await ctx.BroadcastEmails
                .AsNoTracking()
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            var broadcastEmailsDTO = _mapper.Map<List<BroadcastEmail>, List<BroadcastEmailDTO>>(broadcastEmails);

            broadcastEmailsDTO.ForEach(be =>
            {
                be.DateSentStr = be.DateSent.ToString("M/d/yyyy");
            });

            return broadcastEmailsDTO;
        }

        public async Task<bool> OkToSend()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var workOptIn = await ctx.WorkOptIn.FirstOrDefaultAsync();

            if (workOptIn == null)
            {
                WorkOptIn wOptIn = new WorkOptIn();

                wOptIn.RefreshDate = DateTime.Now.AddHours(-2);

                await ctx.WorkOptIn.AddAsync(wOptIn);

                await ctx.SaveChangesAsync();

                return true;
            }
            else if (workOptIn.RefreshDate.AddHours(2) < DateTime.Now)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
