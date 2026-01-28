namespace ColbyRJ.Repository
{
    public class TripRepository : ITripRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IFileUpload _fileUpload;
        private readonly IUtilityRepository _utility;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;

        public TripRepository(
            IDbContextFactory<ApplicationDbContext> ctxFactory,
            IFileUpload fileUpload,
            IUtilityRepository utility,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContext,
            IMapper mapper)
        {
            _ctxFactory = ctxFactory;
            _fileUpload = fileUpload;
            _utility = utility;
            _userManager = userManager;
            _httpContext = httpContext;
            _mapper = mapper;
        }

        public async Task<List<TripDTO>> GetTrips()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var trips = await ctx.Trips
                .Include(q => q.TripGroups)
                .Include(q => q.TripPhotos.Where(q => q.TripGroupId == null))
                .Include(q => q.Comments.OrderByDescending(a => a.Id))
                .Where(q => q.Owner == appUser.DisplayName || appUser.Role == "Admin")
                .AsNoTracking()
                .OrderBy(a => a.YearMonthInt).ThenBy(a => a.Title)
                .ToListAsync();

            var tripsDTO = _mapper.Map<List<Trip>, List<TripDTO>>(trips);

            tripsDTO.ForEach(s =>
            {
                if (s.Active)
                {
                    s.ActiveStr = "yes";
                }

                if (s.TripPhotos.Count > 0)
                {
                    s.PhotoCount = s.TripPhotos.Count.ToString();
                }

                if (s.Comments.Count > 0)
                {
                    s.CommentCount = s.Comments.Count.ToString();
                }
            });

            return tripsDTO;
        }

        public async Task<List<TripDTO>> GetActiveTrips()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var trips = await ctx.Trips
                .Where(q => q.Active == true)
                .AsNoTracking()
                .OrderBy(a => a.YearMonthInt).ThenBy(a => a.Title)
                .ToListAsync();

            var tripsDTO = _mapper.Map<List<Trip>, List<TripDTO>>(trips);

            tripsDTO.ForEach(s =>
            {
                s.Decade = s.YearStr.Substring(0, 3) + "0s";
            });

            return tripsDTO;
        }

        public async Task<TripDTO> GetTrip(int tripId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var trip = await ctx.Trips
                .Include(a => a.TripGroups)
                .Include(a => a.TripPhotos.Where(q => q.TripGroupId == null))
                .Include(q => q.Comments.OrderByDescending(q => q.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == tripId);

            var tripDTO = _mapper.Map<Trip, TripDTO>(trip);

            if (tripDTO.YearInt > 0)
            {
                tripDTO.YearStr = tripDTO.YearInt.ToString();
            }
            if (tripDTO.MonthInt > 0)
            {
                tripDTO.MonthStr = tripDTO.MonthInt.ToString();
            }

            return tripDTO;
        }

        public async Task<TripDTO> GetTripByKey(string key)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var trip = await ctx.Trips
                .Include(a => a.TripGroups)
                .Include(a => a.TripPhotos.Where(q => q.TripGroupId == null))
                .Include(q => q.Comments.OrderByDescending(q => q.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Key == key);

            var tripDTO = _mapper.Map<Trip, TripDTO>(trip);

            if (tripDTO.YearInt > 0)
            {
                tripDTO.YearStr = tripDTO.YearInt.ToString();
            }
            if (tripDTO.MonthInt > 0)
            {
                tripDTO.MonthStr = tripDTO.MonthInt.ToString();
            }

            return tripDTO;
        }

        public async Task<string> CreateTrip(TripCreateDTO tripDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var category = "New";
            var groupedBy = "New";

            var yearStr = tripDTO.YearInt.ToString();
            var monStr = tripDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            int yearInt = (int)tripDTO.YearInt;
            int monthInt = 0;
            if (monStr != "" && monStr != "0")
            {
                monthInt = Convert.ToInt32(monStr.ToString());
            }

            var trip = new Trip
            {
                Title = tripDTO.Title,

                Category = "New",
                GroupedBy = "New",
                YearStr = yearStr,
                MonStr = monStr,
                YearMon = yearMon,
                YearMonth = yearMonth,
                Element = "Trip",
                YearInt = yearInt,
                YearMonthInt = yearInt,
                Note = string.Empty,
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                DateUpdated = DateTime.Now
            };

            if (monthInt > 0)
            {
                trip.MonthInt = monthInt;
                trip.YearMonthInt = Convert.ToInt32(yearInt.ToString() + monthInt.ToString());
                if (monthInt < 10)
                {
                    trip.YearMonthInt = Convert.ToInt32(yearInt.ToString() + "0" + monthInt.ToString());
                }
            }

            ctx.Trips.Add(trip);
            await ctx.SaveChangesAsync();

            Random rnd = new Random();
            var key = trip.Id.ToString() + "-" + rnd.Next(trip.Id * 7, trip.Id * 123).ToString();

            trip.Key = key;

            ctx.Trips.Update(trip);
            await ctx.SaveChangesAsync();

            return "id-" + trip.Id.ToString();
        }

        public async Task<int> DeleteTrip(int tripId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var trip = await ctx.Trips
                .Include(q => q.TripPhotos)
                .FirstOrDefaultAsync(q => q.Id == tripId);

            if (trip.TripPhotos != null && trip.TripPhotos.Count > 0)
            {
                foreach (var item in trip.TripPhotos)
                {
                    var photoUrl = item.PhotoUrl.ToLower();
                    var photoName = photoUrl.Replace($"tripphotos/", "");
                    _fileUpload.DeleteFile(photoName, "tripPhotos");
                }
            }

            ctx.Trips.Remove(trip);
            return await ctx.SaveChangesAsync();
        }

        public async Task<TripDTO> UpdateTrip(TripDTO tripDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var trip = await ctx.Trips
                .FirstOrDefaultAsync(t => t.Id == tripDTO.Id);

            var yearInt = Convert.ToInt32(tripDTO.YearInt);
            int monthInt = 0;
            if (tripDTO.MonthStr != null && tripDTO.MonthStr.Length > 0)
            {
                monthInt = Convert.ToInt32(tripDTO.MonthStr);
            }

            trip.Title = tripDTO.Title;
            trip.DateUpdated = DateTime.Now;
            trip.YearInt = tripDTO.YearInt;
            trip.YearMonthInt = yearInt;
            trip.Note = tripDTO.Note;
            trip.Active = tripDTO.Active;

            if (monthInt > 0)
            {
                trip.MonthInt = monthInt;
                trip.YearMonthInt = Convert.ToInt32(yearInt.ToString() + monthInt.ToString());
                if (monthInt < 10)
                {
                    trip.YearMonthInt = Convert.ToInt32(yearInt.ToString() + "0" + monthInt.ToString());
                }
            }

            ctx.Trips.Update(trip);
            await ctx.SaveChangesAsync();

            return _mapper.Map<Trip, TripDTO>(trip);
        }

        public async Task<string> UpdateGroupBy(GroupByEditDTO groupByDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var trip = await ctx.Trips
                .FirstOrDefaultAsync(t => t.Id == groupByDTO.Id);

            var category = groupByDTO.Category;
            var section = groupByDTO.Section.ToString();
            var topic = groupByDTO.Topic.ToString();
            var groupedBy = await _utility.GetGroupedBy(category, section, topic);

            trip.Category = groupByDTO.Category;
            trip.Section = groupByDTO.Section.ToString();
            trip.Topic = groupByDTO.Topic.ToString();
            trip.GroupedBy = groupedBy;
            trip.DateUpdated = DateTime.Now;

            ctx.Trips.Update(trip);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<string> UpdateYearMon(YearMonEditDTO yearMonDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var trip = await ctx.Trips
                .FirstOrDefaultAsync(t => t.Id == yearMonDTO.Id);

            var yearStr = yearMonDTO.YearInt.ToString();
            var monStr = yearMonDTO.MonStr.ToString();
            var yearMon = await _utility.GetYearMon(yearStr, monStr);
            var yearMonth = await _utility.GetYearMonth(yearStr, monStr);

            int yearInt = (int)yearMonDTO.YearInt;
            int monthInt = 0;
            if (monStr != "" || monStr != "0")
            {
                monthInt = Convert.ToInt32(monStr.ToString());
            }

            trip.YearStr = yearStr;
            trip.YearInt = yearInt;
            trip.MonStr = monStr;
            trip.MonthInt = monthInt;
            trip.YearMon = yearMon;
            trip.YearMonth = yearMonth;
            trip.DateUpdated = DateTime.Now;

            ctx.Trips.Update(trip);
            await ctx.SaveChangesAsync();

            return "ok";
        }

        public async Task<List<TripGroupDTO>> GetTripGroups()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var groups = await ctx.TripGroups
                .Include(q => q.Trip)
                .Include(q => q.TripSections)
                .Include(q => q.TripPhotos.Where(q => q.TripSectionId == null))
                .AsNoTracking()
                .OrderBy(a => a.Sort).ThenBy(a => a.Name)
                .ToListAsync();

            var groupsDTO = _mapper.Map<List<TripGroup>, List<TripGroupDTO>>(groups);

            groupsDTO.ForEach(s =>
            {
                if (s.TripPhotos.Count > 0)
                {
                    s.PhotoCount = s.TripPhotos.Count.ToString();
                }
            });

            return groupsDTO;
        }

        public async Task<string> CreateGroup(TripGroupCreateDTO groupDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var group = new TripGroup
            {
                TripId = groupDTO.TripId,
                Sort = 99,
                Name = string.Empty,
                Note = string.Empty,
                DateUpdated = DateTime.Now
            };

            ctx.TripGroups.Add(group);
            await ctx.SaveChangesAsync();

            return "id-" + group.Id.ToString();
        }

        public async Task<int> DeleteTripGroup(int groupId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var group = await ctx.TripGroups
                .Include(q => q.TripPhotos)
                .FirstOrDefaultAsync(q => q.Id == groupId);

            if (group.TripPhotos != null && group.TripPhotos.Count > 0)
            {
                foreach (var item in group.TripPhotos)
                {
                    var photoUrl = item.PhotoUrl.ToLower();
                    var photoName = photoUrl.Replace($"tripphotos/", "");
                    _fileUpload.DeleteFile(photoName, "tripPhotos");
                }
            }

            ctx.TripGroups.Remove(group);
            return await ctx.SaveChangesAsync();
        }

        public async Task<TripGroupDTO> GetGroup(int groupId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var group = await ctx.TripGroups
                .Include(q => q.Trip)
                .Include(a => a.TripSections)
                .Include(a => a.TripPhotos.Where(q => q.TripSectionId == null))
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == groupId);

            var groupDTO = _mapper.Map<TripGroup, TripGroupDTO>(group);

            return groupDTO;
        }

        public async Task<TripGroupDTO> UpdateGroup(TripGroupDTO groupDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var group = await ctx.TripGroups
                .FirstOrDefaultAsync(t => t.Id == groupDTO.Id);

            group.Name = groupDTO.Name;
            group.Sort = groupDTO.Sort;
            group.DateUpdated = DateTime.Now;
            group.Note = groupDTO.Note;

            ctx.TripGroups.Update(group);
            await ctx.SaveChangesAsync();

            return _mapper.Map<TripGroup, TripGroupDTO>(group);
        }


        public async Task<List<TripSectionDTO>> GetTripSections()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var sections = await ctx.TripSections
                .Include(q => q.TripGroup)
                .Include(q => q.TripSubSections)
                .Include(q => q.TripPhotos.Where(q => q.TripSubSectionId == null))
                .AsNoTracking()
                .OrderBy(a => a.Sort).ThenBy(a => a.Name)
                .ToListAsync();

            var sectionsDTO = _mapper.Map<List<TripSection>, List<TripSectionDTO>>(sections);

            sectionsDTO.ForEach(s =>
            {
                if (s.TripPhotos.Count > 0)
                {
                    s.PhotoCount = s.TripPhotos.Count.ToString();
                }
            });

            return sectionsDTO;
        }

        public async Task<string> CreateSection(TripSectionCreateDTO sectionDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var section = new TripSection
            {
                TripGroupId = sectionDTO.TripGroupId,
                Sort = 99,
                Name = string.Empty,
                Note = string.Empty,
                DateUpdated = DateTime.Now
            };

            ctx.TripSections.Add(section);
            await ctx.SaveChangesAsync();

            return "id-" + section.Id.ToString();
        }

        public async Task<int> DeleteTripSection(int sectionId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var section = await ctx.TripSections
                .Include(q => q.TripPhotos)
                .FirstOrDefaultAsync(q => q.Id == sectionId);

            if (section.TripPhotos != null && section.TripPhotos.Count > 0)
            {
                foreach (var item in section.TripPhotos)
                {
                    var photoUrl = item.PhotoUrl.ToLower();
                    var photoName = photoUrl.Replace($"tripphotos/", "");
                    _fileUpload.DeleteFile(photoName, "tripPhotos");
                }
            }

            ctx.TripSections.Remove(section);
            return await ctx.SaveChangesAsync();
        }

        public async Task<TripSectionDTO> GetSection(int sectionId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var section = await ctx.TripSections
                .Include(q => q.TripGroup)
                .Include(a => a.TripSubSections)
                .Include(a => a.TripPhotos.Where(q => q.TripSubSectionId == null))
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == sectionId);

            var sectionDTO = _mapper.Map<TripSection, TripSectionDTO>(section);

            return sectionDTO;
        }

        public async Task<TripSectionDTO> UpdateSection(TripSectionDTO sectionDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var section = await ctx.TripSections
                .FirstOrDefaultAsync(t => t.Id == sectionDTO.Id);

            section.Name = sectionDTO.Name;
            section.Sort = sectionDTO.Sort;
            section.DateUpdated = DateTime.Now;
            section.Note = sectionDTO.Note;

            ctx.TripSections.Update(section);
            await ctx.SaveChangesAsync();

            return _mapper.Map<TripSection, TripSectionDTO>(section);
        }


        public async Task<List<TripSubSectionDTO>> GetTripSubSections()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var subSections = await ctx.TripSubSections
                .Include(q => q.TripSection)
                .Include(q => q.TripPhotos)
                .AsNoTracking()
                .OrderBy(a => a.Sort).ThenBy(a => a.Name)
                .ToListAsync();

            var subSectionsDTO = _mapper.Map<List<TripSubSection>, List<TripSubSectionDTO>>(subSections);

            subSectionsDTO.ForEach(s =>
            {
                if (s.TripPhotos.Count > 0)
                {
                    s.PhotoCount = s.TripPhotos.Count.ToString();
                }
            });

            return subSectionsDTO;
        }

        public async Task<string> CreateSubSection(TripSubSectionCreateDTO subSectionDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var subSection = new TripSubSection
            {
                TripSectionId = subSectionDTO.TripSectionId,
                Sort = 99,
                Name = string.Empty,
                Note = string.Empty,
                DateUpdated = DateTime.Now
            };

            ctx.TripSubSections.Add(subSection);
            await ctx.SaveChangesAsync();

            return "id-" + subSection.Id.ToString();
        }

        public async Task<int> DeleteTripSubSection(int subSectionId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var subSection = await ctx.TripSubSections
                .Include(q => q.TripPhotos)
                .FirstOrDefaultAsync(q => q.Id == subSectionId);

            if (subSection.TripPhotos != null && subSection.TripPhotos.Count > 0)
            {
                foreach (var item in subSection.TripPhotos)
                {
                    var photoUrl = item.PhotoUrl.ToLower();
                    var photoName = photoUrl.Replace($"tripphotos/", "");
                    _fileUpload.DeleteFile(photoName, "tripPhotos");
                }
            }

            ctx.TripSubSections.Remove(subSection);
            return await ctx.SaveChangesAsync();
        }

        public async Task<TripSubSectionDTO> GetSubSection(int subSectionId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var subSection = await ctx.TripSubSections
                .Include(q => q.TripSection)
                .Include(a => a.TripPhotos)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == subSectionId);

            var subSectionDTO = _mapper.Map<TripSubSection, TripSubSectionDTO>(subSection);

            return subSectionDTO;
        }

        public async Task<TripSubSectionDTO> UpdateSubSection(TripSubSectionDTO subSectionDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var subSection = await ctx.TripSubSections
                .FirstOrDefaultAsync(t => t.Id == subSectionDTO.Id);

            subSection.Name = subSectionDTO.Name;
            subSection.Sort = subSectionDTO.Sort;
            subSection.DateUpdated = DateTime.Now;
            subSection.Note = subSectionDTO.Note;

            ctx.TripSubSections.Update(subSection);
            await ctx.SaveChangesAsync();

            return _mapper.Map<TripSubSection, TripSubSectionDTO>(subSection);
        }


        public async Task<List<TripPhotoDTO>> GetAllTripPhotos()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photos = await ctx.TripPhotos
                .Include(q => q.Trip)
                .Include(q => q.TripGroup)
                .Include(q => q.TripSection)
                .Include(q => q.TripSubSection)
                .AsNoTracking()
                .OrderBy(a => a.Sort).ThenBy(a => a.Caption)
                .ToListAsync();

            var photosDTO = _mapper.Map<List<TripPhoto>, List<TripPhotoDTO>>(photos);

            return photosDTO;
        }

        public async Task<List<TripPhotoDTO>> GetTripPhotos(int tripId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photos = await ctx.TripPhotos
                .Include(q => q.Trip)
                .Include(q => q.TripGroup)
                .Include(q => q.TripSection)
                .Include(q => q.TripSubSection)
                .Where(q => q.TripId == tripId)
                .AsNoTracking()
                .OrderBy(a => a.Sort).ThenBy(a => a.Caption)
                .ToListAsync();

            var photosDTO = _mapper.Map<List<TripPhoto>, List<TripPhotoDTO>>(photos);

            return photosDTO;
        }

        public async Task<List<TripPhotoDTO>> GetGroupPhotos(int groupId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photos = await ctx.TripPhotos
                .Include(q => q.Trip)
                .Include(q => q.TripGroup)
                .Include(q => q.TripSection)
                .Include(q => q.TripSubSection)
                .Where(q => q.TripGroupId == groupId && q.TripSectionId == null)
                .AsNoTracking()
                .OrderBy(a => a.Sort).ThenBy(a => a.Caption)
                .ToListAsync();

            var photosDTO = _mapper.Map<List<TripPhoto>, List<TripPhotoDTO>>(photos);

            return photosDTO;
        }

        public async Task<List<TripPhotoDTO>> GetSectionPhotos(int sectionId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photos = await ctx.TripPhotos
                .Include(q => q.Trip)
                .Include(q => q.TripGroup)
                .Include(q => q.TripSection)
                .Include(q => q.TripSubSection)
                .Where(q => q.TripSectionId == sectionId && q.TripSubSectionId == null)
                .AsNoTracking()
                .OrderBy(a => a.Sort).ThenBy(a => a.Caption)
                .ToListAsync();

            var photosDTO = _mapper.Map<List<TripPhoto>, List<TripPhotoDTO>>(photos);

            return photosDTO;
        }

        public async Task<List<TripPhotoDTO>> GetSubSectionPhotos(int subSectionId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photos = await ctx.TripPhotos
                .Include(q => q.Trip)
                .Include(q => q.TripGroup)
                .Include(q => q.TripSection)
                .Include(q => q.TripSubSection)
                .Where(q => q.TripSubSectionId == subSectionId)
                .AsNoTracking()
                .OrderBy(a => a.Sort).ThenBy(a => a.Caption)
                .ToListAsync();

            var photosDTO = _mapper.Map<List<TripPhoto>, List<TripPhotoDTO>>(photos);

            return photosDTO;
        }

        public async Task<int> DeletePhotoByPhotoUrl(string photoUrl)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var allPhotos = await ctx.TripPhotos.FirstOrDefaultAsync
                                (x => x.PhotoUrl.ToLower() == photoUrl.ToLower());
            if (allPhotos == null)
            {
                return 0;
            }
            ctx.TripPhotos.Remove(allPhotos);
            return await ctx.SaveChangesAsync();
        }

        public async Task<int> DeletePhoto(int photoId)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var photo = await ctx.TripPhotos.FirstOrDefaultAsync(x => x.Id == photoId);


            var photoUrl = photo.PhotoUrl;
            var photoName = photoUrl.Replace($"tripPhotos/", "");

            var result = _fileUpload.DeleteFile(photoName, "tripPhotos");

            ctx.TripPhotos.Remove(photo);
            return await ctx.SaveChangesAsync();
        }

        public async Task<string> CreatePhoto(TripPhotoDTO photoDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photo = new TripPhoto
            {
                Caption = photoDTO.Caption,
                TripId = photoDTO.TripId,
                PhotoUrl = photoDTO.PhotoUrl,
                Sort = 99,
                DateUpdated = DateTime.Now
            };

            if (photoDTO.TripGroupId != null)
            {
                photo.TripGroupId = photoDTO.TripGroupId;
            }
            if (photoDTO.TripSectionId != null)
            {
                photo.TripSectionId = photoDTO.TripSectionId;
            }
            if (photoDTO.TripSubSectionId != null)
            {
                photo.TripSubSectionId = photoDTO.TripSubSectionId;
            }

            await ctx.TripPhotos.AddAsync(photo);
            await ctx.SaveChangesAsync();

            return "id-" + photo.Id.ToString();
        }

        public async Task<TripPhotoDTO> GetPhoto(int photoId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photo = await ctx.TripPhotos
                .Include(q => q.Trip)
                .AsNoTracking()
               .FirstOrDefaultAsync(q => q.Id == photoId);

            var photoDTO = _mapper.Map<TripPhoto, TripPhotoDTO>(photo);

            return photoDTO;
        }

        public async Task<string> UpdatePhoto(TripPhotoDTO photoDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photo = await ctx.TripPhotos.FirstOrDefaultAsync(q => q.Id == photoDTO.Id);

            photo.Caption = photoDTO.Caption;
            photo.Sort = photoDTO.Sort;
            photo.DateUpdated = DateTime.Now;

            ctx.TripPhotos.Update(photo);
            await ctx.SaveChangesAsync();

            return "ok";
        }


        public async Task<string> CreateComment(TripCommentDTO commentDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var appUser = await ctx.AppUsers.FirstOrDefaultAsync(q => q.Email == user.Email);

            var comment = new TripComment
            {
                Comments = commentDTO.Comments,
                TripId = commentDTO.TripId,
                Owner = appUser.DisplayName,
                OwnerEmail = appUser.Email,
                CommentDate = DateTime.Now
            };

            ctx.TripComments.Add(comment);
            await ctx.SaveChangesAsync();

            return "id-" + comment.Id.ToString();
        }

        public async Task<IEnumerable<TripCommentDTO>> GetComments(int tripId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comments = await ctx.TripComments
                .Include(q => q.Trip)
                .Where(q => q.TripId == tripId)
              .OrderByDescending(q => q.Id)
              .AsNoTracking().ToListAsync();

            var commentsDTO = _mapper.Map<IEnumerable<TripComment>, IEnumerable<TripCommentDTO>>(comments);
            return commentsDTO;
        }

        public async Task<int> DeleteComment(int commentId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var comment = await ctx.TripComments.FirstOrDefaultAsync(q => q.Id == commentId);
            if (comment != null)
            {
                ctx.TripComments.Remove(comment);
                return await ctx.SaveChangesAsync();
            }

            return 0;
        }


    }
}
