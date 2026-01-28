namespace ColbyRJ.Repository
{
    public class WhoAmICommentRepository : IWhoAmICommentRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public WhoAmICommentRepository(
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

        public async Task<string> Create(WhoAmICommentDTO commentDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var comment = new WhoAmIComment
            {
                Comments = commentDTO.Comments,
                WhoAmIOwner = commentDTO.WhoAmIOwner,
                WhoAmIOwnerEmail = commentDTO.WhoAmIOwnerEmail,
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                CommentDate = DateTime.Now
            };

            ctx.WhoAmIComments.Add(comment);
            await ctx.SaveChangesAsync();

            return "id-" + comment.Id.ToString();
        }

        public async Task<int> Delete(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.WhoAmIComments.FirstOrDefaultAsync(q => q.Id == commentId);
            if (comment != null)
            {
                ctx.WhoAmIComments.Remove(comment);
                return await ctx.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<IEnumerable<WhoAmICommentDTO>> GetAllComments()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comments = await ctx.WhoAmIComments
                .OrderBy(q => q.WhoAmIOwnerEmail)
              .ThenByDescending(q => q.Id)
              .AsNoTracking().ToListAsync();

            var commentsDTO = _mapper.Map<IEnumerable<WhoAmIComment>, IEnumerable<WhoAmICommentDTO>>(comments);
            return commentsDTO;
        }

        public async Task<WhoAmICommentDTO> GetComment(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.WhoAmIComments
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == commentId);

            var commentDTO = _mapper.Map<WhoAmIComment, WhoAmICommentDTO>(comment);
            return commentDTO;
        }

        public async Task<IEnumerable<WhoAmICommentDTO>> GetComments(string ownerEmail)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comments = await ctx.WhoAmIComments
                .Where(q => q.WhoAmIOwnerEmail == ownerEmail)
              .OrderByDescending(q => q.Id)
              .AsNoTracking().ToListAsync();

            var commentsDTO = _mapper.Map<IEnumerable<WhoAmIComment>, IEnumerable<WhoAmICommentDTO>>(comments);
            return commentsDTO;
        }

        public async Task<IEnumerable<WhoAmICommentDTO>> GetOwnerComments(string owner)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comments = await ctx.WhoAmIComments
                .Where(q => q.WhoAmIOwner == owner)
              .OrderByDescending(q => q.Id)
              .AsNoTracking().ToListAsync();

            var commentsDTO = _mapper.Map<IEnumerable<WhoAmIComment>, IEnumerable<WhoAmICommentDTO>>(comments);
            return commentsDTO;
        }
    }
}
