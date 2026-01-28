namespace ColbyRJ.Repository
{
    public class StoryChapterCommentRepository : IStoryChapterCommentRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public StoryChapterCommentRepository(
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

        public async Task<string> Create(StoryChapterCommentDTO commentDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var comment = new StoryChapterComment
            {
                Comments = commentDTO.Comments,
                StoryChapterId = commentDTO.StoryChapterId,
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                CommentDate = DateTime.Now
            };

            ctx.StoryChapterComments.Add(comment);
            await ctx.SaveChangesAsync();

            return "id-" + comment.Id.ToString();
        }

        public async Task<int> Delete(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.StoryChapterComments.FirstOrDefaultAsync(q => q.Id == commentId);
            if (comment != null)
            {
                ctx.StoryChapterComments.Remove(comment);
                return await ctx.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<StoryChapterCommentDTO> GetComment(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.StoryChapterComments
                .Include(q => q.StoryChapter)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == commentId);

            var commentDTO = _mapper.Map<StoryChapterComment, StoryChapterCommentDTO>(comment);
            return commentDTO;
        }

        public async Task<IEnumerable<StoryChapterCommentDTO>> GetComments(int chapterId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comments = await ctx.StoryChapterComments
                .Include(q => q.StoryChapter)
                .Where(q => q.StoryChapterId == chapterId)
              .OrderByDescending(q => q.Id)
              .AsNoTracking().ToListAsync();

            var commentsDTO = _mapper.Map<IEnumerable<StoryChapterComment>, IEnumerable<StoryChapterCommentDTO>>(comments);
            return commentsDTO;
        }

    }
}
