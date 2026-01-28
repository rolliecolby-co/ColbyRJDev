namespace ColbyRJ.Repository.IRepository
{
    public interface IOptInSendRepository
    {
        public Task<string> SendOptIn();
        public Task<ICollection<OptInSendToDTO>> GetOptInSendTo();
        public Task<ICollection<OptInSendToDTO>> GetOptInEmails();
        public Task<string> GetSatStatus();
    }
}
