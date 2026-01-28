namespace ColbyRJ.Mapper
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<AddressHistory, AddressDTO>().ReverseMap();
            CreateMap<AddressComment, AddressCommentDTO>().ReverseMap();
            CreateMap<AddressPhoto, AddressPhotoDTO>().ReverseMap();
            CreateMap<AppUser, AppUserDTO>().ReverseMap();
            CreateMap<BroadcastEmail, BroadcastEmailDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Doc, DocDTO>().ReverseMap();
            CreateMap<DocComment, DocCommentDTO>().ReverseMap();
            CreateMap<DocPdf, DocPdfDTO>().ReverseMap();
            CreateMap<GalleryDecade, GalleryDecadeDTO>().ReverseMap();
            CreateMap<GalleryPhoto, GalleryPhotoDTO>().ReverseMap();
            CreateMap<GallerySection, GallerySectionDTO>().ReverseMap();
            CreateMap<Hint, HintDTO>().ReverseMap();
            CreateMap<JobHistory, JobDTO>().ReverseMap();
            CreateMap<JobComment, JobCommentDTO>().ReverseMap();
            CreateMap<LoginRecord, LoginRecordDTO>().ReverseMap();
            CreateMap<PhotoAlbum, PhotoAlbumDTO>().ReverseMap();
            CreateMap<PhotoAlbumComment, PhotoAlbumCommentDTO>().ReverseMap();
            CreateMap<PhotoAlbumPhoto, PhotoAlbumPhotoDTO>().ReverseMap();
            CreateMap<PhotoFolder, PhotoFolderDTO>().ReverseMap();
            CreateMap<PhotoFolderComment, PhotoFolderCommentDTO>().ReverseMap();
            CreateMap<Section, SectionDTO>().ReverseMap();
            CreateMap<Snippet, SnippetDTO>().ReverseMap();
            CreateMap<SnippetComment, SnippetCommentDTO>().ReverseMap();
            CreateMap<SnippetPhoto, SnippetPhotoDTO>().ReverseMap();
            CreateMap<Story, StoryDTO>().ReverseMap();
            CreateMap<StoryChapter, StoryChapterDTO>().ReverseMap();
            CreateMap<StoryChapterPhoto, StoryChapterPhotoDTO>().ReverseMap();
            CreateMap<StoryChapterComment, StoryChapterCommentDTO>().ReverseMap();
            CreateMap<StoryComment, StoryCommentDTO>().ReverseMap();
            CreateMap<StoryPhoto, StoryPhotoDTO>().ReverseMap();
            CreateMap<Topic, TopicDTO>().ReverseMap();
            CreateMap<Trip, TripDTO>().ReverseMap();
            CreateMap<TripComment, TripCommentDTO>().ReverseMap();
            CreateMap<TripGroup, TripGroupDTO>().ReverseMap();
            CreateMap<TripPhoto, TripPhotoDTO>().ReverseMap();
            CreateMap<TripSection, TripSectionDTO>().ReverseMap();
            CreateMap<TripSubSection, TripSubSectionDTO>().ReverseMap();
            CreateMap<UnitHistory, UnitDTO>().ReverseMap();
            CreateMap<UnitComment, UnitCommentDTO>().ReverseMap();
            CreateMap<Video, VideoDTO>().ReverseMap();
            CreateMap<VideoComment, VideoCommentDTO>().ReverseMap();
            CreateMap<WhoAmI, WhoAmIDTO>().ReverseMap();
            CreateMap<WhoAmIComment, WhoAmICommentDTO>().ReverseMap();

            CreateMap<AddressCommentDTO, CommentDTO>();
            CreateMap<DocCommentDTO, CommentDTO>();
            CreateMap<JobCommentDTO, CommentDTO>();
            CreateMap<PhotoAlbumCommentDTO, CommentDTO>();
            CreateMap<PhotoFolderCommentDTO, CommentDTO>();
            CreateMap<SnippetCommentDTO, CommentDTO>();
            CreateMap<StoryCommentDTO, CommentDTO>();
            CreateMap<StoryChapterCommentDTO, CommentDTO>();
            CreateMap<TripCommentDTO, CommentDTO>();
            CreateMap<UnitCommentDTO, CommentDTO>();
            CreateMap<VideoCommentDTO, CommentDTO>();
            CreateMap<WhoAmICommentDTO, CommentDTO>();
        }
    }
}
