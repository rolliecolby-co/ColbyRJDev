namespace ColbyRJ.Repository.IRepository
{
    public interface IJobRepository
    {
        public Task<List<JobDTO>> GetJobs();
        public Task<List<JobDTO>> GetBrowseJobs();
        public Task<int> Delete(int jobId);
        public Task<string> Create(JobDTO jobDTO);
        public Task<JobDTO> GetJob(int jobId);
        public Task<string> Update(JobDTO jobDTO);
        public Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO);
    }
}
