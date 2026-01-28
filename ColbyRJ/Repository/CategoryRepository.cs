namespace ColbyRJ.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;

        public CategoryRepository(
            IDbContextFactory<ApplicationDbContext> ctxFactory,
            IMapper mapper)
        {
            _ctxFactory = ctxFactory;
            _mapper = mapper;
        }

        public async Task<string> Create(CategoryAddDTO categoryDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var category = new Category
            {
                Name = categoryDTO.Name
            };

            ctx.Categories.Add(category);
            await ctx.SaveChangesAsync();

            return "id-" + category.Id.ToString();
        }

        public async Task<int> Delete(int categoryId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var category = await ctx.Categories.FirstOrDefaultAsync(t => t.Id == categoryId);

            ctx.Categories.Remove(category);
            return await ctx.SaveChangesAsync();
        }

        public async Task<List<CategoryDTO>> GetCategories()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var categories = await ctx.Categories
                .Include(c => c.Sections)
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

            var categoriesDTO = _mapper.Map<List<Category>, List<CategoryDTO>>(categories);

            return categoriesDTO;
        }

        public async Task<List<CategoryDTO>> GetCategoriesFull()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var categories = await ctx.Categories
                .Include(c => c.Sections.OrderBy(s => s.Name))
                .ThenInclude(s => s.Topics.OrderBy(t => t.Name))
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

            var categoriesDTO = _mapper.Map<List<Category>, List<CategoryDTO>>(categories);

            return categoriesDTO;
        }

        public async Task<CategoryDTO> GetCategory(int categoryId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var category = await ctx.Categories
                .Include(c => c.Sections)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == categoryId);

            if (category == null)
            {
                return null;
            }

            var categoryDTO = _mapper.Map<Category, CategoryDTO>(category);

            return categoryDTO;
        }

        public async Task<string> Update(CategoryDTO categoryDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var category = await ctx.Categories
                .FirstOrDefaultAsync(t => t.Id == categoryDTO.Id);

            category.Name = categoryDTO.Name;

            ctx.Categories.Update(category);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
