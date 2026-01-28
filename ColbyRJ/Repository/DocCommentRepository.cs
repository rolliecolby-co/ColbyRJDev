namespace ColbyRJ.Repository
{
    public class DocCommentRepository : IDocCommentRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public DocCommentRepository(
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

        public async Task<string> Create(DocCommentDTO commentDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var comment = new DocComment
            {
                Comments = commentDTO.Comments,
                DocId = commentDTO.DocId,
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                CommentDate = DateTime.Now
            };

            ctx.DocComments.Add(comment);
            await ctx.SaveChangesAsync();

            return "id-" + comment.Id.ToString();
        }

        public async Task<int> Delete(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.DocComments.FirstOrDefaultAsync(q => q.Id == commentId);
            if (comment != null)
            {
                ctx.DocComments.Remove(comment);
                return await ctx.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<DocCommentDTO> GetComment(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.DocComments
                .Include(q => q.Doc)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == commentId);

            var commentDTO = _mapper.Map<DocComment, DocCommentDTO>(comment);
            return commentDTO;
        }

        public async Task<IEnumerable<DocCommentDTO>> GetComments(int docId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comments = await ctx.DocComments
                .Include(q => q.Doc)
                .Where(q => q.DocId == docId)
              .OrderByDescending(q => q.Id)
              .AsNoTracking().ToListAsync();

            var commentsDTO = _mapper.Map<IEnumerable<DocComment>, IEnumerable<DocCommentDTO>>(comments);
            return commentsDTO;
        }
    }
}
