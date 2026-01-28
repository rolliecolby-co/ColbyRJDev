namespace ColbyRJ.Repository.IRepository
{
    public interface IUtilityRepository
    {
        public Task<string> GetGroupedBy(string category, string section, string topic);
        public Task<string> GetYearMon(string yearStr, string monStr);
        public Task<string> GetYearMonth(string yearStr, string monStr);
    }
}
