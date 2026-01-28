namespace ColbyRJ.Repository.IRepository
{
    public interface IWhoAmIRepository
    {
        public Task<List<WhoAmIDTO>> GetWhoAmIs();
        public Task<List<WhoAmIDTO>> GetAllWhoAmIs();
        public Task<List<WhoAmIDTO>> GetBrowseWhoAmIs();
        public Task<int> Delete(int whoAmIId);
        public Task<string> Create(WhoAmIDTO whoAmIDTO);
        public Task<WhoAmIDTO> GetWhoAmI(int whoAmIId);
        public Task<WhoAmIDTO> GetWhoAmIByOwner(string owner);
        public Task<string> Update(WhoAmIDTO whoAmIDTO);
    }
}
