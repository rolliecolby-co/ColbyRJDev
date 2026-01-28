namespace ColbyRJ.Repository.IRepository
{
    public interface IBroadcastEmailRepository
    {
        public Task<List<BroadcastEmailDTO>> GetBroadcastEmails();
        public Task<int> Delete(int broadcastEmailId);
        public Task<string> Create(BroadcastEmailDTO broadcastEmailDTO);
        public Task<bool> OkToSend();
        public Task<BroadcastEmailDTO> GetBroadcastEmail(int broadcastEmailId);
    }
}
