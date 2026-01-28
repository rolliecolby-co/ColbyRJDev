namespace ColbyRJ.Repository.IRepository
{
    public interface IDocRepository
    {
        public Task<string> Create(DocCreateDTO docDTO);
        public Task<int> Delete(int docId);
        public Task<List<DocDTO>> GetDocs();
        public Task<List<DocDTO>> GetActiveDocs();
        public Task<DocDTO> GetDoc(int docId);
        public Task<DocDTO> GetDocByKey(string key);
        public Task<DocDTO> Update(DocDTO docDTO);
        public Task<string> UpdateGroupBy(GroupByEditDTO groupByDTO);
        public Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO);
    }
}
