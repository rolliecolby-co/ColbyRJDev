namespace ColbyRJ.Repository
{
    public class SnippetCommentRepository : ISnippetCommentRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public SnippetCommentRepository(
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

        public async Task<string> Create(SnippetCommentDTO commentDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var comment = new SnippetComment
            {
                Comments = commentDTO.Comments,
                SnippetId = commentDTO.SnippetId,
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                CommentDate = DateTime.Now
            };

            ctx.SnippetComments.Add(comment);
            await ctx.SaveChangesAsync();

            return "id-" + comment.Id.ToString();
        }

        public async Task<int> Delete(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.SnippetComments.FirstOrDefaultAsync(q => q.Id == commentId);
            if (comment != null)
            {
                ctx.SnippetComments.Remove(comment);
                return await ctx.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<SnippetCommentDTO> GetComment(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.SnippetComments
                .Include(q => q.Snippet)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == commentId);

            var commentDTO = _mapper.Map<SnippetComment, SnippetCommentDTO>(comment);
            return commentDTO;
        }

        public async Task<IEnumerable<SnippetCommentDTO>> GetComments(int snippetId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comments = await ctx.SnippetComments
                .Include(q => q.Snippet)
                .Where(q => q.SnippetId == snippetId)
              .OrderByDescending(q => q.Id)
              .AsNoTracking().ToListAsync();

            var commentsDTO = _mapper.Map<IEnumerable<SnippetComment>, IEnumerable<SnippetCommentDTO>>(comments);
            return commentsDTO;
        }

    }
}
