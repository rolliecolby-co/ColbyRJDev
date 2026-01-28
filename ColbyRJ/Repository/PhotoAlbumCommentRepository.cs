namespace ColbyRJ.Repository
{
    public class PhotoAlbumCommentRepository : IPhotoAlbumCommentRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public PhotoAlbumCommentRepository(
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

        public async Task<string> Create(PhotoAlbumCommentDTO commentDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var comment = new PhotoAlbumComment
            {
                Comments = commentDTO.Comments,
                PhotoAlbumId = commentDTO.PhotoAlbumId,
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                CommentDate = DateTime.Now
            };

            ctx.PhotoAlbumComments.Add(comment);
            await ctx.SaveChangesAsync();

            return "id-" + comment.Id.ToString();
        }

        public async Task<int> Delete(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.PhotoAlbumComments.FirstOrDefaultAsync(q => q.Id == commentId);
            if (comment != null)
            {
                ctx.PhotoAlbumComments.Remove(comment);
                return await ctx.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<PhotoAlbumCommentDTO> GetComment(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.PhotoAlbumComments
                .Include(q => q.PhotoAlbum)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == commentId);

            var commentDTO = _mapper.Map<PhotoAlbumComment, PhotoAlbumCommentDTO>(comment);
            return commentDTO;
        }

        public async Task<IEnumerable<PhotoAlbumCommentDTO>> GetComments(int photoAlbumId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comments = await ctx.PhotoAlbumComments
                .Include(q => q.PhotoAlbum)
                .Where(q => q.PhotoAlbumId == photoAlbumId)
              .OrderByDescending(q => q.Id)
              .AsNoTracking().ToListAsync();

            var commentsDTO = _mapper.Map<IEnumerable<PhotoAlbumComment>, IEnumerable<PhotoAlbumCommentDTO>>(comments);
            return commentsDTO;
        }

    }
}
