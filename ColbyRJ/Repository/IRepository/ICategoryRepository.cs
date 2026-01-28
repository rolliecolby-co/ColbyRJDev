namespace ColbyRJ.Repository.IRepository
{
    public interface ICategoryRepository
    {
        public Task<List<CategoryDTO>> GetCategories();
        public Task<List<CategoryDTO>> GetCategoriesFull();
        public Task<int> Delete(int categoryId);
        public Task<string> Create(CategoryAddDTO categoryDTO);
        public Task<CategoryDTO> GetCategory(int categoryId);
        public Task<string> Update(CategoryDTO categoryDTO);
    }
}
