namespace ColbyRJ.Repository
{
    public class BaseRepository : IBaseRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;
        private readonly string rootPath = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\photoDirs\"}";
        private readonly string rootFolder = "/photoDirs/";

        public BaseRepository(
            IDbContextFactory<ApplicationDbContext> ctxFactory,
            IMapper mapper)
        {
            _ctxFactory = ctxFactory;
            _mapper = mapper;
        }

        public async Task<BrowseTally> GetBrowseTally()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            BrowseTally tally = new BrowseTally();

            tally.PhotoAlbumCount = ctx.PhotoAlbums
                    .Where(q => q.Active == true).Count();
            tally.PhotoFolderCount = ctx.PhotoFolders
                    .Where(q => q.Active == true).Count();
            tally.SnippetCount = ctx.Snippets
                    .Where(q => q.Active == true).Count();
            tally.StoryCount = ctx.Stories
                    .Where(q => q.Active == true).Count();
            tally.VideoCount = ctx.Videos
                    .Where(q => q.Active == true).Count();

            tally.AllCount = tally.PhotoAlbumCount + tally.PhotoFolderCount
                + tally.SnippetCount + tally.StoryCount + tally.VideoCount;

            tally.AddressCount = ctx.Addresses.Count();
            tally.JobCount = ctx.Jobs.Count();
            tally.UnitCount = ctx.Units.Count();
            tally.WhoAmICount = ctx.WhoAmI.Count();

            return tally;
        }

        public async Task<BrowseDTO> GetBrowsing(string byWhat)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            BrowseDTO browseDTO = new BrowseDTO();
            List<string> groups = new List<string>();
            List<string> decadesStr = new List<string>();
            List<string> strings = new List<string>();
            List<GroupByDTO> groupedBy = new List<GroupByDTO>();
            List<GroupByElementDTO> groupedByElements = new List<GroupByElementDTO>();
            List<YearMonDTO> yearMons = new List<YearMonDTO>();
            List<DecadeDTO> decades = new List<DecadeDTO>();
            List<DecadeElementDTO> decadeElements = new List<DecadeElementDTO>();

            if (byWhat == "all" || byWhat == "photoAlbums")
            {
                var photoAlbums = await ctx.PhotoAlbums
                    .Where(q => q.Active == true)
                    .ToListAsync();
                browseDTO.PhotoAlbums = photoAlbums;
                var photoAlbumsDTO = _mapper.Map<List<PhotoAlbum>, List<PhotoAlbumDTO>>(photoAlbums);

                strings = photoAlbums.Select(pa => pa.GroupedBy).ToList();
                groups.AddRange(strings);

                strings = photoAlbums.Select(pa => pa.YearStr.Substring(0, 3) + "0s").ToList();
                decadesStr.AddRange(strings);

                var paGroupByElements = photoAlbums.GroupBy(s => new { s.GroupedBy, s.Element }, (key, group) => new GroupByElementDTO()
                {
                    GroupBy = key.GroupedBy,
                    Element = key.Element,
                    GBECount = group.Count()
                }).ToList();
                groupedByElements.AddRange(paGroupByElements);

                photoAlbumsDTO.ForEach(s =>
                {
                    s.Decade = s.YearStr.Substring(0, 3) + "0s";
                });

                var paDecadeElements = photoAlbumsDTO.GroupBy(s => new { s.Decade, s.Element }, (key, group) => new DecadeElementDTO()
                {
                    Decade = key.Decade,
                    Element = key.Element,
                    DECount = group.Count()
                }).ToList();
                decadeElements.AddRange(paDecadeElements);
            }

            if (byWhat == "all" || byWhat == "photoFolders")
            {
                var photoFolders = await ctx.PhotoFolders
                    .Where(q => q.Active == true)
                    .ToListAsync();
                browseDTO.PhotoFolders = photoFolders;
                var photoFoldersDTO = _mapper.Map<List<PhotoFolder>, List<PhotoFolderDTO>>(photoFolders);

                strings = photoFolders.Select(pf => pf.GroupedBy).ToList();
                groups.AddRange(strings);

                strings = photoFolders.Select(pf => pf.YearStr.Substring(0, 3) + "0s").ToList();
                decadesStr.AddRange(strings);

                var pfGroupByElements = photoFolders.GroupBy(s => new { s.GroupedBy, s.Element }, (key, group) => new GroupByElementDTO()
                {
                    GroupBy = key.GroupedBy,
                    Element = key.Element,
                    GBECount = group.Count()
                }).ToList();
                groupedByElements.AddRange(pfGroupByElements);

                photoFoldersDTO.ForEach(s =>
                {
                    s.Decade = s.YearStr.Substring(0, 3) + "0s";
                });

                var pfDecadeElements = photoFoldersDTO.GroupBy(s => new { s.Decade, s.Element }, (key, group) => new DecadeElementDTO()
                {
                    Decade = key.Decade,
                    Element = key.Element,
                    DECount = group.Count()
                }).ToList();
                decadeElements.AddRange(pfDecadeElements);
            }

            if (byWhat == "all" || byWhat == "snippets")
            {
                var snippets = await ctx.Snippets
                    .Where(q => q.Active == true)
                    .ToListAsync();
                browseDTO.Snippets = snippets;
                var snippetsDTO = _mapper.Map<List<Snippet>, List<SnippetDTO>>(snippets);

                strings = snippets.Select(s => s.GroupedBy).ToList();
                groups.AddRange(strings);

                strings = snippets.Select(s => s.YearStr.Substring(0, 3) + "0s").ToList();
                decadesStr.AddRange(strings);

                var sGroupByElements = snippets.GroupBy(s => new { s.GroupedBy, s.Element }, (key, group) => new GroupByElementDTO()
                {
                    GroupBy = key.GroupedBy,
                    Element = key.Element,
                    GBECount = group.Count()
                }).ToList();
                groupedByElements.AddRange(sGroupByElements);

                snippetsDTO.ForEach(s =>
                {
                    s.Decade = s.YearStr.Substring(0, 3) + "0s";
                });

                var sDecadeElements = snippetsDTO.GroupBy(s => new { s.Decade, s.Element }, (key, group) => new DecadeElementDTO()
                {
                    Decade = key.Decade,
                    Element = key.Element,
                    DECount = group.Count()
                }).ToList();
                decadeElements.AddRange(sDecadeElements);
            }

            if (byWhat == "all" || byWhat == "stories")
            {
                var stories = await ctx.Stories
                    .Where(q => q.Active == true)
                    .ToListAsync();
                browseDTO.Stories = stories;
                var storiesDTO = _mapper.Map<List<Story>, List<StoryDTO>>(stories);

                strings = stories.Select(s => s.GroupedBy).ToList();
                groups.AddRange(strings);

                strings = stories.Select(s => s.YearStr.Substring(0, 3) + "0s").ToList();
                decadesStr.AddRange(strings);

                var stGroupByElements = stories.GroupBy(s => new { s.GroupedBy, s.Element }, (key, group) => new GroupByElementDTO()
                {
                    GroupBy = key.GroupedBy,
                    Element = key.Element,
                    GBECount = group.Count()
                }).ToList();
                groupedByElements.AddRange(stGroupByElements);

                storiesDTO.ForEach(s =>
                {
                    s.Decade = s.YearStr.Substring(0, 3) + "0s";
                });

                var stDecadeElements = storiesDTO.GroupBy(s => new { s.Decade, s.Element }, (key, group) => new DecadeElementDTO()
                {
                    Decade = key.Decade,
                    Element = key.Element,
                    DECount = group.Count()
                }).ToList();
                decadeElements.AddRange(stDecadeElements);
            }

            if (byWhat == "all" || byWhat == "videos")
            {
                var videos = await ctx.Videos
                    .Where(q => q.Active == true)
                    .ToListAsync();
                browseDTO.Videos = videos;
                var videosDTO = _mapper.Map<List<Video>, List<VideoDTO>>(videos);

                strings = videos.Select(v => v.GroupedBy).ToList();
                groups.AddRange(strings);

                strings = videos.Select(v => v.YearStr.Substring(0, 3) + "0s").ToList();
                decadesStr.AddRange(strings);

                var vGroupByElements = videos.GroupBy(s => new { s.GroupedBy, s.Element }, (key, group) => new GroupByElementDTO()
                {
                    GroupBy = key.GroupedBy,
                    Element = key.Element,
                    GBECount = group.Count()
                }).ToList();
                groupedByElements.AddRange(vGroupByElements);

                videosDTO.ForEach(s =>
                {
                    s.Decade = s.YearStr.Substring(0, 3) + "0s";
                });

                var vDecadeElements = videosDTO.GroupBy(s => new { s.Decade, s.Element }, (key, group) => new DecadeElementDTO()
                {
                    Decade = key.Decade,
                    Element = key.Element,
                    DECount = group.Count()
                }).ToList();
                decadeElements.AddRange(vDecadeElements);
            }

            var groupByGroups = groups.GroupBy(g => g)
                .Select(g => new GroupByDTO()
                {
                    GroupBy = g.Key,
                    GBCount = g.Count()
                })
                .ToList();

            // add groupByGroups to browseDTO.GroupedBy
            groupedBy = groupByGroups.OrderBy(g => g.GroupBy).ToList();
            browseDTO.GroupedBy = groupedBy.ToList();

            var decadeGroups = decadesStr.GroupBy(y => y)
                .Select(y => new DecadeDTO()
                {
                    Decade = y.Key,
                    DecadeCount = y.Count()
                })
                .ToList();

            // add decadeGroups to browseDTO.Decades
            decades = decadeGroups.OrderByDescending(y => y.Decade).ToList();
            browseDTO.Decades = decades.ToList();

            groupedByElements = groupedByElements.OrderBy(g => g.GroupBy).ThenBy(g => g.Element).ToList();
            browseDTO.GroupedByElements = groupedByElements;

            decadeElements = decadeElements.OrderByDescending(g => g.Decade).ThenBy(g => g.Element).ToList();
            browseDTO.DecadeElements = decadeElements;

            return browseDTO;
        }

        public async Task<List<ElementDTO>> GetElements()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            List<ElementDTO> Elements = new List<ElementDTO>();
            int photosCount = 0;
            int cnt = 0;

            var docs = await ctx.Docs
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.YearMon).ThenBy(s => s.Title)
                .AsNoTracking()
                .ToListAsync();

            docs.ForEach(d =>
            {
                var element = new ElementDTO
                {
                    GroupedBy = d.GroupedBy,
                    ElementOrder = "1",
                    Element = "Document",
                    Owner = d.Owner,
                    YearMon = d.YearMon,
                    YearMonth = d.YearMonth,
                    Title = d.Title
                };
                Elements.Add(element);
            });

            var photoAlbums = await ctx.PhotoAlbums
                .Include(x => x.Photos)
                .AsNoTracking()
                .ToListAsync();

            photoAlbums.ForEach(pa =>
            {
                photosCount = 0;
                if (pa.Photos != null)
                {
                    photosCount = pa.Photos.Count;
                }

                var element = new ElementDTO
                {
                    GroupedBy = pa.GroupedBy,
                    ElementOrder = "2",
                    Element = "Photo Album",
                    Owner = pa.Owner,
                    YearMon = pa.YearMon,
                    YearMonth = pa.YearMonth,
                    Title = pa.Title,
                    PhotoCount = photosCount.ToString()
                };
                Elements.Add(element);
            });

            var photoFolders = await ctx.PhotoFolders
                .AsNoTracking()
                .ToListAsync();

            photoFolders.ForEach(pf =>
            {
                var photoCount = 0;
                if (pf.PathFolder.Length > 0)
                {
                    var pathFolder = pf.PathFolder;
                    var pathDir = pathFolder.Replace("/", "\\");
                    var path = rootPath + pathDir;
                    var files = Directory.GetFiles(path);
                    foreach (var file in files)
                    {
                        photoCount++;
                    }
                    photosCount = photoCount;
                }

                var element = new ElementDTO
                {
                    GroupedBy = pf.GroupedBy,
                    ElementOrder = "3",
                    Element = "Photo Folder",
                    Owner = pf.Owner,
                    YearMon = pf.YearMon,
                    YearMonth = pf.YearMonth,
                    Title = pf.Title,
                    PhotoCount = photosCount.ToString()
                };
                Elements.Add(element);
            });

            var snippets = await ctx.Snippets
                .Include(x => x.Photos)
                .AsNoTracking()
                .ToListAsync();

            snippets.ForEach(s =>
            {
                photosCount = 0;
                if (s.Photos != null)
                {
                    photosCount = s.Photos.Count;
                }

                var element = new ElementDTO
                {
                    GroupedBy = s.GroupedBy,
                    ElementOrder = "4",
                    Element = "Snippet",
                    Owner = s.Owner,
                    YearMon = s.YearMon,
                    YearMonth = s.YearMonth,
                    Title = s.Title,
                    PhotoCount = photosCount.ToString()
                };
                Elements.Add(element);
            });

            var stories = await ctx.Stories
                .Include(s => s.Photos)
                .Include(s => s.Chapters.OrderBy(c => c.YearMon))
                    .ThenInclude(c => c.Photos)
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.YearMon).ThenBy(s => s.Title)
                .ToListAsync();

            cnt = 0;
            stories.ForEach(s =>
            {
                cnt++;
                photosCount = 0;
                if (s.Photos != null)
                {
                    photosCount = s.Photos.Count;
                }

                var element = new ElementDTO
                {
                    GroupedBy = s.GroupedBy,
                    ElementOrder = "5 - " + cnt.ToString(),
                    Element = "Story",
                    Owner = s.Owner,
                    YearMon = s.YearMon,
                    YearMonth = s.YearMonth,
                    Title = s.Title,
                    PhotoCount = photosCount.ToString()
                };
                Elements.Add(element);

                if (s.Chapters != null && s.Chapters.Count > 0)
                {
                    var chapters = s.Chapters.ToList();
                    chapters.OrderBy(q => q.YearMon).ThenBy(q => q.Title);

                    chapters.ForEach(ch =>
                    {
                        cnt++;
                        photosCount = 0;
                        if (ch.Photos != null)
                        {
                            photosCount = ch.Photos.Count;
                        }

                        var element = new ElementDTO
                        {
                            GroupedBy = s.GroupedBy,
                            ElementOrder = "5 - " + cnt.ToString(),
                            Element = "Chapter",
                            YearMon = ch.YearMon,
                            YearMonth = ch.YearMonth,
                            Title = ch.Title,
                            PhotoCount = photosCount.ToString()
                        };
                        Elements.Add(element);
                    });

                }
            });

            var videos = await ctx.Videos
                .AsNoTracking()
                .ToListAsync();

            videos.ForEach(v =>
            {
                var element = new ElementDTO
                {
                    GroupedBy = v.GroupedBy,
                    ElementOrder = "6",
                    Element = "Video",
                    Owner = v.Owner,
                    YearMon = v.YearMon,
                    YearMonth = v.YearMonth,
                    Title = v.Title
                };
                Elements.Add(element);
            });

            Elements = Elements
                .OrderBy(q => q.GroupedBy)
                .ThenBy(q => q.ElementOrder)
                .ThenBy(q => q.YearMon)
                .ThenBy(q => q.Title)
                .ToList();

            return Elements;
        }

        public async Task<ElementTally> GetElementTally()
        {
            using var ctx = _ctxFactory.CreateDbContext();
            ElementTally elementTally = new ElementTally();

            var photoAlbums = await ctx.PhotoAlbums
                    .Where(q => q.Active == true)
                    .ToListAsync();

            elementTally.PhotoAlbumCount = photoAlbums.Count();

            var photoFolders = await ctx.PhotoFolders
                    .Where(q => q.Active == true)
                    .ToListAsync();

            elementTally.PhotoFolderCount = photoFolders.Count();

            var snippets = await ctx.Snippets
                    .Where(q => q.Active == true)
                    .ToListAsync();

            elementTally.SnippetCount = snippets.Count();

            var stories = await ctx.Stories
                    .Where(q => q.Active == true)
                    .ToListAsync();

            elementTally.StoryCount = stories.Count();

            var videos = await ctx.Videos
                    .Where(q => q.Active == true)
                    .ToListAsync();

            elementTally.VideoCount = videos.Count();
            return elementTally;
        }

        public async Task<List<PhotoAlbumDTO>> GetPhotoAlbums(string groupedBy)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoAlbums = await ctx.PhotoAlbums
                .Where(pa => pa.Active == true && pa.GroupedBy == groupedBy)
                .OrderBy(pa => pa.Title)
                .ToListAsync();

            var photoAlbumsDTO = _mapper.Map<List<PhotoAlbum>, List<PhotoAlbumDTO>>(photoAlbums);

            return photoAlbumsDTO;
        }

        public async Task<List<PhotoAlbumDTO>> GetPhotoAlbums()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoAlbums = await ctx.PhotoAlbums
                .Where(pa => pa.Active == true)
                .OrderBy(pa => pa.GroupedBy).ThenBy(pa => pa.Title)
                .ToListAsync();

            var photoAlbumsDTO = _mapper.Map<List<PhotoAlbum>, List<PhotoAlbumDTO>>(photoAlbums);

            return photoAlbumsDTO;
        }

        public async Task<List<PhotoAlbumDTO>> GetPhotoAlbumsByDecade(string decadeStr)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoAlbums = await ctx.PhotoAlbums
                .Where(pa => pa.Active == true && pa.YearStr.Substring(0, 3) == decadeStr.Substring(0, 3))
                .OrderBy(pa => pa.Title)
                .ToListAsync();

            var photoAlbumsDTO = _mapper.Map<List<PhotoAlbum>, List<PhotoAlbumDTO>>(photoAlbums);

            return photoAlbumsDTO;
        }

        public async Task<List<PhotoFolderDTO>> GetPhotoFolders(string groupedBy)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoFolders = await ctx.PhotoFolders
                .Where(pf => pf.Active == true && pf.GroupedBy == groupedBy)
                .OrderBy(pf => pf.Title)
                .ToListAsync();

            var photoFoldersDTO = _mapper.Map<List<PhotoFolder>, List<PhotoFolderDTO>>(photoFolders);

            return photoFoldersDTO;
        }

        public async Task<List<PhotoFolderDTO>> GetPhotoFolders()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoFolders = await ctx.PhotoFolders
                .Where(pf => pf.Active == true)
                .OrderBy(pf => pf.GroupedBy).ThenBy(pf => pf.Title)
                .ToListAsync();

            var photoFoldersDTO = _mapper.Map<List<PhotoFolder>, List<PhotoFolderDTO>>(photoFolders);

            return photoFoldersDTO;
        }

        public async Task<List<PhotoFolderDTO>> GetPhotoFoldersByDecade(string decadeStr)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var photoFolders = await ctx.PhotoFolders
                .Where(pf => pf.Active == true && pf.YearStr.Substring(0, 3) == decadeStr.Substring(0, 3))
                .OrderBy(pf => pf.Title)
                .ToListAsync();

            var photoFoldersDTO = _mapper.Map<List<PhotoFolder>, List<PhotoFolderDTO>>(photoFolders);

            return photoFoldersDTO;
        }

        public async Task<List<SnippetDTO>> GetSnippets(string groupedBy)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var snippets = await ctx.Snippets
                .Where(s => s.Active == true && s.GroupedBy == groupedBy)
                .OrderBy(s => s.Title)
                .ToListAsync();

            var snippetsDTO = _mapper.Map<List<Snippet>, List<SnippetDTO>>(snippets);

            return snippetsDTO;
        }

        public async Task<List<SnippetDTO>> GetSnippets()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var snippets = await ctx.Snippets
                .Where(s => s.Active == true)
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.Title)
                .ToListAsync();

            var snippetsDTO = _mapper.Map<List<Snippet>, List<SnippetDTO>>(snippets);

            return snippetsDTO;
        }

        public async Task<List<SnippetDTO>> GetSnippetsByDecade(string decadeStr)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var snippets = await ctx.Snippets
                .Where(s => s.Active == true && s.YearStr.Substring(0, 3) == decadeStr.Substring(0, 3))
                .OrderBy(s => s.Title)
                .ToListAsync();

            var snippetsDTO = _mapper.Map<List<Snippet>, List<SnippetDTO>>(snippets);

            return snippetsDTO;
        }

        public async Task<List<StoryDTO>> GetStories(string groupedBy)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var stories = await ctx.Stories
                .Include(s => s.Chapters.Where(c => c.Active == true))
                .Where(s => s.Active == true && s.GroupedBy == groupedBy)
                .OrderBy(s => s.Title)
                .ToListAsync();

            var storiesDTO = _mapper.Map<List<Story>, List<StoryDTO>>(stories);

            storiesDTO.ForEach(s =>
            {
                if (s.Chapters.Count() > 0)
                {
                    s.ChapterCount = "+ " + s.Chapters.Count().ToString() + " Chapter(s)";
                }
            });

            return storiesDTO;
        }

        public async Task<List<StoryDTO>> GetStories()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var stories = await ctx.Stories
                .Include(s => s.Chapters.Where(c => c.Active == true))
                .Where(s => s.Active == true)
                .OrderBy(s => s.GroupedBy).ThenBy(s => s.Title)
                .ToListAsync();

            var storiesDTO = _mapper.Map<List<Story>, List<StoryDTO>>(stories);

            storiesDTO.ForEach(s =>
            {
                if (s.Chapters.Count() > 0)
                {
                    s.ChapterCount = "+ " + s.Chapters.Count().ToString() + " Chapter(s)";
                }
            });

            return storiesDTO;
        }

        public async Task<List<StoryDTO>> GetStoriesByDecade(string decadeStr)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var stories = await ctx.Stories
                .Include(s => s.Chapters.Where(c => c.Active == true))
                .Where(s => s.Active == true && s.YearStr.Substring(0, 3) == decadeStr.Substring(0, 3))
                .OrderBy(s => s.Title)
                .ToListAsync();

            var storiesDTO = _mapper.Map<List<Story>, List<StoryDTO>>(stories);

            storiesDTO.ForEach(s =>
            {
                if (s.Chapters.Count() > 0)
                {
                    s.ChapterCount = "+ " + s.Chapters.Count().ToString() + " Chapter(s)";
                }
            });

            return storiesDTO;
        }

        public async Task<List<VideoDTO>> GetVideos(string groupedBy)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var videos = await ctx.Videos
                .Include(v => v.GroupedBy)
                .Where(v => v.Active == true && v.GroupedBy == groupedBy)
                .OrderBy(v => v.Title)
                .ToListAsync();

            var videosDTO = _mapper.Map<List<Video>, List<VideoDTO>>(videos);

            return videosDTO;
        }

        public async Task<List<VideoDTO>> GetVideos()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var videos = await ctx.Videos
                .Where(v => v.Active == true)
                .OrderBy(v => v.GroupedBy).ThenBy(v => v.Title)
                .ToListAsync();

            var videosDTO = _mapper.Map<List<Video>, List<VideoDTO>>(videos);

            return videosDTO;
        }

        public async Task<List<VideoDTO>> GetVideosByDecade(string decadeStr)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var videos = await ctx.Videos
                .Where(v => v.Active == true && v.YearStr.Substring(0, 3) == decadeStr.Substring(0, 3))
                .OrderBy(v => v.Title)
                .ToListAsync();

            var videosDTO = _mapper.Map<List<Video>, List<VideoDTO>>(videos);

            return videosDTO;
        }
    }
}
