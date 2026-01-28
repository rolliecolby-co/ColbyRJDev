namespace ColbyRJ.Repository.IRepository
{
    public interface ISnippetRepository
    {
        public Task<string> Create(SnippetCreateDTO snippetDTO);
        public Task<int> Delete(int snippetId);
        public Task<List<SnippetDTO>> GetSnippets();
        public Task<List<SnippetDTO>> GetActiveSnippets();
        public Task<SnippetDTO> GetSnippet(int snippetId);
        public Task<SnippetDTO> GetSnippetByKey(string key);
        public Task<SnippetDTO> Update(SnippetDTO snippetDTO);
        public Task<string> UpdateGroupBy(GroupByEditDTO groupByDTO);
        public Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO);
    }
}
