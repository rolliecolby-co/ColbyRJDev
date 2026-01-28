namespace ColbyRJ.Repository
{
    public class JobRepository : IJobRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly IUtilityRepository _utility;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public JobRepository(
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

        public async Task<string> Create(JobDTO jobDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var yearStr = jobDTO.YearInt.ToString();
            var monStr = jobDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            var job = new JobHistory
            {
                //Who = jobDTO.Who,
                Who = appUser.DisplayName,
                //StartDate = jobDTO.StartDate,
                YearStr = yearStr,
                MonStr = monStr,
                YearMon = yearMon,
                YearMonth = yearMonth,
                Remarks = jobDTO.Remarks,
                Employer = jobDTO.Employer,
                Position = jobDTO.Position,
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                DateUpdated = DateTime.Now
            };

            ctx.Jobs.Add(job);
            await ctx.SaveChangesAsync();

            return "id-" + job.Id.ToString();
        }

        public async Task<int> Delete(int jobId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var job = await ctx.Jobs.FirstOrDefaultAsync(t => t.Id == jobId);

            ctx.Jobs.Remove(job);
            return await ctx.SaveChangesAsync();
        }

        public async Task<List<JobDTO>> GetBrowseJobs()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var jobs = await ctx.Jobs
                .Include(j => j.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .OrderBy(a => a.Who)
                .ThenBy(a => a.YearMon)
                .ToListAsync();

            var jobsDTO = _mapper.Map<List<JobHistory>, List<JobDTO>>(jobs);

            jobsDTO.ForEach(j =>
            {
                j.StartDateStr = j.StartDate.ToString("MMM yyyy");
                if (j.Remarks.Length > 0)
                {
                    j.WithRemarks = "yes";
                }

                if (j.Comments.Count > 0)
                {
                    j.CommentCount = j.Comments.Count.ToString();
                }
            });

            return jobsDTO;
        }

        public async Task<JobDTO> GetJob(int jobId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var job = await ctx.Jobs
                .Include(j => j.Comments.OrderByDescending(a => a.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == jobId);

            var jobDTO = _mapper.Map<JobHistory, JobDTO>(job);

            jobDTO.YearInt = Convert.ToInt32(jobDTO.YearStr);
            //jobDTO.StartDateStr = jobDTO.StartDate.ToString("MMM yyyy");

            return jobDTO;
        }

        public async Task<List<JobDTO>> GetJobs()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var jobs = await ctx.Jobs
                .Include(j => j.Comments.OrderByDescending(a => a.Id))
                .Where(q => q.OwnerEmail == appUser.Email || appUser.Role == "Admin")
                .AsNoTracking()
                .OrderBy(a => a.Who)
                .ThenBy(a => a.YearMon)
                .ToListAsync();

            var jobsDTO = _mapper.Map<List<JobHistory>, List<JobDTO>>(jobs);

            jobsDTO.ForEach(j =>
            {
                j.StartDateStr = j.StartDate.ToString("MMM yyyy");
                if (j.Remarks.Length > 0)
                {
                    j.WithRemarks = "yes";
                }

                if (j.Comments.Count > 0)
                {
                    j.CommentCount = j.Comments.Count.ToString() + " Comment(s)";
                }
            });

            return jobsDTO;
        }

        public async Task<string> Update(JobDTO jobDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var job = await ctx.Jobs
                .FirstOrDefaultAsync(q => q.Id == jobDTO.Id);

            //job.Who = jobDTO.Who;
            //job.StartDate = jobDTO.StartDate;
            job.Remarks = jobDTO.Remarks;
            job.Employer = jobDTO.Employer;
            job.Position = jobDTO.Position;
            job.DateUpdated = DateTime.Now;

            ctx.Jobs.Update(job);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var job = await ctx.Jobs
                .FirstOrDefaultAsync(t => t.Id == yearMonDTO.Id);

            var yearStr = yearMonDTO.YearInt.ToString();
            var monStr = yearMonDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            job.YearStr = yearStr;
            job.MonStr = monStr;
            job.YearMon = yearMon;
            job.YearMonth = yearMonth;
            job.DateUpdated = DateTime.Now;

            ctx.Jobs.Update(job);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
