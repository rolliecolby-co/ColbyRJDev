namespace ColbyRJ.Repository.IRepository
{
    public interface ITripRepository
    {
        public Task<List<TripDTO>> GetTrips();
        public Task<List<TripDTO>> GetActiveTrips();
        public Task<TripDTO> GetTrip(int tripId);
        public Task<TripDTO> GetTripByKey(string key);
        public Task<string> CreateTrip(TripCreateDTO tripDTO);
        public Task<int> DeleteTrip(int tripId);
        public Task<TripDTO> UpdateTrip(TripDTO tripDTO);
        public Task<string> UpdateGroupBy(GroupByEditDTO groupByDTO);
        public Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO);

        public Task<List<TripGroupDTO>> GetTripGroups();
        public Task<string> CreateGroup(TripGroupCreateDTO groupDTO);
        public Task<int> DeleteTripGroup(int groupId);
        public Task<TripGroupDTO> GetGroup(int groupId);
        public Task<TripGroupDTO> UpdateGroup(TripGroupDTO groupDTO);

        public Task<List<TripSectionDTO>> GetTripSections();
        public Task<string> CreateSection(TripSectionCreateDTO sectionDTO);
        public Task<int> DeleteTripSection(int sectionId);
        public Task<TripSectionDTO> GetSection(int sectionId);
        public Task<TripSectionDTO> UpdateSection(TripSectionDTO sectionDTO);

        public Task<List<TripSubSectionDTO>> GetTripSubSections();
        public Task<string> CreateSubSection(TripSubSectionCreateDTO subSectionDTO);
        public Task<int> DeleteTripSubSection(int subSectionId);
        public Task<TripSubSectionDTO> GetSubSection(int subSectionId);
        public Task<TripSubSectionDTO> UpdateSubSection(TripSubSectionDTO subSectionDTO);

        public Task<List<TripPhotoDTO>> GetAllTripPhotos();
        public Task<List<TripPhotoDTO>> GetTripPhotos(int tripId);
        public Task<List<TripPhotoDTO>> GetGroupPhotos(int groupId);
        public Task<List<TripPhotoDTO>> GetSectionPhotos(int sectionId);
        public Task<List<TripPhotoDTO>> GetSubSectionPhotos(int subSectionId);
        public Task<string> CreatePhoto(TripPhotoDTO photoDTO);
        public Task<int> DeletePhotoByPhotoUrl(string photoUrl);
        public Task<int> DeletePhoto(int photoId);
        public Task<TripPhotoDTO> GetPhoto(int photoId);
        public Task<string> UpdatePhoto(TripPhotoDTO photoDTO);

        public Task<string> CreateComment(TripCommentDTO commentDTO);
        public Task<IEnumerable<TripCommentDTO>> GetComments(int tripId);
        public Task<int> DeleteComment(int commentId);
    }
}
