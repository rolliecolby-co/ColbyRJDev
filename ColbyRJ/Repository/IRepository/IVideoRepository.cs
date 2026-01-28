namespace ColbyRJ.Repository.IRepository
{
    public interface IVideoRepository
    {
        public Task<string> Create(VideoCreateDTO videoDTO);
        public Task<int> Delete(int videoId);
        public Task<List<VideoDTO>> GetVideos();
        public Task<List<VideoDTO>> GetActiveVideos();
        public Task<VideoDTO> GetVideo(int videoId);
        public Task<VideoDTO> GetVideoByKey(string key);
        public Task<string> Update(VideoDTO videoDTO);
        public Task<string> UpdateGroupBy(GroupByEditDTO groupByDTO);
        public Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO);
        public Task<string> UpdateOwner(OwnerEditDTO ownerDTO);
    }
}
