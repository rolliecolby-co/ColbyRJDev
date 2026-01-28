namespace ColbyRJ.Repository.IRepository
{
    public interface ISectionRepository
    {
        public Task<string> Create(SectionAddDTO sectionDTO);
        public Task<int> Delete(int sectionId);
        public Task<List<SectionDTO>> GetSections();
        public Task<SectionDTO> GetSection(int sectionId);
        public Task<List<string>> GetSectionsCategories();
        public Task<string> Update(SectionDTO sectionDTO);
        public Task<List<string>> GetCategorySectionsStr(string category);
    }
}
