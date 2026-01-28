namespace ColbyRJ.Repository
{
    public class JobCommentRepository : IJobCommentRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public JobCommentRepository(
            IDbContextFactory<ApplicationDbContext> ctxFactory,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContext)
        {
            _ctxFactory = ctxFactory;
            _mapper = mapper;
            _userManager = userManager;
            _httpContext = httpContext;
        }

        public async Task<string> Create(JobCommentDTO commentDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var comment = _mapper.Map<JobCommentDTO, JobComment>(commentDTO);

            comment.Owner = appUser.DisplayName;
            comment.OwnerEmail = appUser.Email;
            comment.CommentDate = DateTime.Now;

            ctx.JobComments.Add(comment);
            await ctx.SaveChangesAsync();

            return "id-" + comment.Id.ToString();
        }

        public async Task<int> Delete(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.JobComments.FirstOrDefaultAsync(q => q.Id == commentId);
            if (comment != null)
            {
                ctx.JobComments.Remove(comment);
                return await ctx.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<JobCommentDTO> GetComment(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.JobComments
                .Include(q => q.Job)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == commentId);

            var commentDTO = _mapper.Map<JobComment, JobCommentDTO>(comment);
            return commentDTO;
        }

        public async Task<IEnumerable<JobCommentDTO>> GetComments(int jobId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comments = await ctx.JobComments
                .Include(q => q.Job)
                .Where(q => q.JobHistoryId == jobId)
              .OrderByDescending(q => q.Id)
              .AsNoTracking().ToListAsync();

            var commentsDTO = _mapper.Map<IEnumerable<JobComment>, IEnumerable<JobCommentDTO>>(comments);
            return commentsDTO;
        }
    }
}
