namespace ColbyRJ.Repository.IRepository
{
    public interface IUnitRepository
    {
        public Task<List<UnitDTO>> GetUnits();
        public Task<List<UnitDTO>> GetBrowseUnits();
        public Task<int> Delete(int unitId);
        public Task<string> Create(UnitDTO unitDTO);
        public Task<UnitDTO> GetUnit(int unitId);
        public Task<string> Update(UnitDTO unitDTO);
        public Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO);
    }
}
