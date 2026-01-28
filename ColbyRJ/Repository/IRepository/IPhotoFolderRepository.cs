namespace ColbyRJ.Repository.IRepository
{
    public interface IPhotoFolderRepository
    {
        public Task<string> Create(PhotoFolderCreateDTO photoFolderDTO);
        public Task<int> Delete(int photoFolderId);
        public Task<List<PhotoFolderDTO>> GetPhotoFolders();
        public Task<List<PhotoFolderDTO>> GetActivePhotoFolders();
        public Task<PhotoFolderDTO> GetPhotoFolder(int photoFolderId);
        public Task<PhotoFolderDTO> GetPhotoFolderByKey(string key);
        public Task<string> Update(PhotoFolderDTO photoFolderDTO);
        public Task<string> UpdateGroupBy(GroupByEditDTO groupByDTO);
        public Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO);
        public Task<string> UpdateOwner(OwnerEditDTO ownerDTO);
    }
}
