namespace ColbyRJ.Repository.IRepository
{
    public interface IHintRepository
    {
        public Task<string> Create(HintDTO hintDTO);
        public Task<List<HintDTO>> GetHints();
        public Task<List<HintDTO>> GetHintsByKey(string key);
        public Task<int> Delete(int hintId);
        public Task<HintDTO> GetHint(int hintId);
        public Task<string> Update(HintDTO hintDTO);
    }
}
