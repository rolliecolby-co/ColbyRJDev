namespace ColbyRJ.Repository
{
    public class PhotoFolderCommentRepository : IPhotoFolderCommentRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public PhotoFolderCommentRepository(
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

        public async Task<string> Create(PhotoFolderCommentDTO commentDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var comment = new PhotoFolderComment
            {
                Comments = commentDTO.Comments,
                PhotoFolderId = commentDTO.PhotoFolderId,
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                CommentDate = DateTime.Now
            };

            ctx.PhotoFolderComments.Add(comment);
            await ctx.SaveChangesAsync();

            return "id-" + comment.Id.ToString();
        }

        public async Task<int> Delete(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.PhotoFolderComments.FirstOrDefaultAsync(q => q.Id == commentId);
            if (comment != null)
            {
                ctx.PhotoFolderComments.Remove(comment);
                return await ctx.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<PhotoFolderCommentDTO> GetComment(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.PhotoFolderComments
                .Include(q => q.PhotoFolder)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == commentId);

            var commentDTO = _mapper.Map<PhotoFolderComment, PhotoFolderCommentDTO>(comment);
            return commentDTO;
        }

        public async Task<IEnumerable<PhotoFolderCommentDTO>> GetComments(int photoFolderId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comments = await ctx.PhotoFolderComments
                .Include(q => q.PhotoFolder)
                .Where(q => q.PhotoFolderId == photoFolderId)
              .OrderByDescending(q => q.Id)
              .AsNoTracking().ToListAsync();

            var commentsDTO = _mapper.Map<IEnumerable<PhotoFolderComment>, IEnumerable<PhotoFolderCommentDTO>>(comments);
            return commentsDTO;
        }

    }
}
