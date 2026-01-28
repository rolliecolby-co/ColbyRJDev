namespace ColbyRJ.Repository
{
    public class AddressCommentRepository : IAddressCommentRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public AddressCommentRepository(
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

        public async Task<string> Create(AddressCommentDTO commentDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var comment = _mapper.Map<AddressCommentDTO, AddressComment>(commentDTO);

            comment.Owner = appUser.DisplayName;
            comment.OwnerEmail = appUser.Email;
            comment.CommentDate = DateTime.Now;

            ctx.AddressComments.Add(comment);
            await ctx.SaveChangesAsync();

            return "id-" + comment.Id.ToString();
        }

        public async Task<int> Delete(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.AddressComments.FirstOrDefaultAsync(q => q.Id == commentId);
            if (comment != null)
            {
                ctx.AddressComments.Remove(comment);
                return await ctx.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<AddressCommentDTO> GetComment(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.AddressComments
                .Include(q => q.Address)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == commentId);

            var commentDTO = _mapper.Map<AddressComment, AddressCommentDTO>(comment);
            return commentDTO;
        }

        public async Task<IEnumerable<AddressCommentDTO>> GetComments(int addressId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comments = await ctx.AddressComments
                .Include(q => q.Address)
                .Where(q => q.AddressHistoryId == addressId)
              .OrderByDescending(q => q.Id)
              .AsNoTracking().ToListAsync();

            var commentsDTO = _mapper.Map<IEnumerable<AddressComment>, IEnumerable<AddressCommentDTO>>(comments);
            return commentsDTO;
        }

    }
}
