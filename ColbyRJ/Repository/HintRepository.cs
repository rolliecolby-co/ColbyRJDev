namespace ColbyRJ.Repository
{
    public class HintRepository : IHintRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;

        public HintRepository(
            IDbContextFactory<ApplicationDbContext> ctxFactory,
            IMapper mapper)
        {
            _ctxFactory = ctxFactory;
            _mapper = mapper;
        }

        public async Task<string> Create(HintDTO hintDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var hint = new Hint
            {
                Key = hintDTO.Key,
                OrderBy = hintDTO.OrderBy,
                Title = hintDTO.Title,
                Value = hintDTO.Value
            };

            ctx.Hints.Add(hint);
            await ctx.SaveChangesAsync();

            return "id-" + hint.Id.ToString();
        }

        public async Task<int> Delete(int hintId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var hint = await ctx.Hints.FirstOrDefaultAsync(a => a.Id == hintId);

            ctx.Hints.Remove(hint);
            return await ctx.SaveChangesAsync();
        }

        public async Task<HintDTO> GetHint(int hintId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var hint = await ctx.Hints
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == hintId);

            if (hint == null)
            {
                return null;
            }

            var hintDTO = _mapper.Map<Hint, HintDTO>(hint);

            return hintDTO;
        }

        public async Task<List<HintDTO>> GetHints()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var hints = await ctx.Hints
                .AsNoTracking()
                .OrderBy(a => a.Key).ThenBy(a => a.OrderBy).ThenBy(a => a.Title)
                .ToListAsync();

            if (hints == null)
            {
                return null;
            }

            var hintsDTO = _mapper.Map<List<Hint>, List<HintDTO>>(hints);

            hintsDTO.ForEach(a =>
            {
                if (a.Key.StartsWith("A"))
                {
                    a.Page = "Admin";
                }
                if (a.Key.StartsWith("B"))
                {
                    a.Page = "Browse";
                }
                if (a.Key.StartsWith("E"))
                {
                    a.Page = "Post / Edit";
                }
            });

            return hintsDTO;
        }

        public async Task<List<HintDTO>> GetHintsByKey(string key)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var hints = await ctx.Hints
                .Where(q => q.Key == key)
                .AsNoTracking()
                .OrderBy(a => a.Key).ThenBy(a => a.OrderBy).ThenBy(a => a.Title)
                .ToListAsync();

            if (hints == null)
            {
                return null;
            }

            var hintsDTO = _mapper.Map<List<Hint>, List<HintDTO>>(hints);

            return hintsDTO;
        }

        public async Task<string> Update(HintDTO hintDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var hint = await ctx.Hints
                .FirstOrDefaultAsync(a => a.Id == hintDTO.Id);

            hint.Key = hintDTO.Key;
            hint.Title = hintDTO.Title;
            hint.Value = hintDTO.Value;
            hint.OrderBy = hintDTO.OrderBy;

            ctx.Hints.Update(hint);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
