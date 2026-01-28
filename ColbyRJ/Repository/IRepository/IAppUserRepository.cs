namespace ColbyRJ.Repository.IRepository
{
    public interface IAppUserRepository
    {
        public Task<string> Create(AppUserCreateDTO appUserDTO);
        public Task<int> Delete(int appUserId);
        public Task<List<AppUserDTO>> GetUsers();
        public Task<AppUserDTO> GetUser(int appUserId);
        public Task<List<LoginTallyDTO>> GetLoginTally();
        public Task<AppUserDTO> GetUserByEmail(string email);
        public Task<string> Update(AppUserDTO appUserDTO);
        public Task<List<WhoDTO>> GetWho(string indAll);
        public Task<List<OwnerDTO>> GetOwners();
    }
}
