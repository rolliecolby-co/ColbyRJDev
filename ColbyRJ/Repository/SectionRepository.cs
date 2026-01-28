namespace ColbyRJ.Repository
{
    public class SectionRepository : ISectionRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;

        public SectionRepository(
            IDbContextFactory<ApplicationDbContext> ctxFactory,
            IMapper mapper)
        {
            _ctxFactory = ctxFactory;
            _mapper = mapper;
        }

        public async Task<string> Create(SectionAddDTO sectionDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var section = new Section
            {
                Name = sectionDTO.Name,
                CategoryId = sectionDTO.CategoryId
            };

            ctx.Sections.Add(section);
            await ctx.SaveChangesAsync();

            return "id-" + section.Id.ToString();
        }

        public async Task<int> Delete(int sectionId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var section = await ctx.Sections.FirstOrDefaultAsync(t => t.Id == sectionId);

            ctx.Sections.Remove(section);
            return await ctx.SaveChangesAsync();
        }

        public async Task<List<string>> GetCategorySectionsStr(string category)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var vCategory = await ctx.Categories
                .Include(c => c.Sections)
                .Where(c => c.Name == category)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            List<Section> sections = new List<Section>();

            List<string> result = new List<string>();

            if (vCategory != null && vCategory.Sections != null && vCategory.Sections.Count > 0)
            {
                sections = vCategory.Sections.OrderBy(s => s.Name).ToList();

                sections.ForEach(s =>
                {
                    result.Add(s.Name);
                });
            }

            return result;
        }

        public async Task<SectionDTO> GetSection(int sectionId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var section = await ctx.Sections
                .Include(s => s.Topics)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == sectionId);

            if (section == null)
            {
                return null;
            }

            var sectionDTO = _mapper.Map<Section, SectionDTO>(section);

            return sectionDTO;
        }

        public async Task<List<SectionDTO>> GetSections()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var sections = await ctx.Sections
                .AsNoTracking()
                .Include(s => s.Category)
                .Include(s => s.Topics.OrderBy(t => t.Name))
                .OrderBy(s => s.Category.Name).ThenBy(s => s.Name)
            .ToListAsync();

            var sectionsDTO = _mapper.Map<List<Section>, List<SectionDTO>>(sections);

            return sectionsDTO;
        }

        public async Task<List<string>> GetSectionsCategories()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var sections = await ctx.Sections
                .AsNoTracking()
                .Include(s => s.Category)
                .OrderBy(s => s.Category.Name)
            .ToListAsync();

            List<string> result = new List<string>();

            sections.ForEach(s =>
                {
                    result.Add(s.Category.Name);
                });

            result = result.AsQueryable().Distinct().ToList();

            return result;
        }

        public async Task<string> Update(SectionDTO sectionDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var section = await ctx.Sections
                .FirstOrDefaultAsync(s => s.Id == sectionDTO.Id);

            section.Name = sectionDTO.Name;
            section.CategoryId = sectionDTO.CategoryId;

            ctx.Sections.Update(section);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
