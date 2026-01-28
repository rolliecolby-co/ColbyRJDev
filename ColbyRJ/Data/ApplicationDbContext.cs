using ColbyRJ.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ColbyRJ.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : IdentityDbContext<ApplicationUser>(options)
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<AddressHistory> Addresses { get; set; }
        public DbSet<AddressComment> AddressComments { get; set; }
        public DbSet<AddressPhoto> AddressPhotos { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<BroadcastEmail> BroadcastEmails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Doc> Docs { get; set; }
        public DbSet<DocComment> DocComments { get; set; }
        public DbSet<DocPdf> DocPdfs { get; set; }
        public DbSet<GalleryDecade> GalleryDecades { get; set; }
        public DbSet<GalleryPhoto> GalleryPhotos { get; set; }
        public DbSet<GallerySection> GallerySections { get; set; }
        public DbSet<Hint> Hints { get; set; }
        public DbSet<JobHistory> Jobs { get; set; }
        public DbSet<JobComment> JobComments { get; set; }
        public DbSet<LoginRecord> LoginRecord { get; set; }
        public DbSet<PhotoAlbum> PhotoAlbums { get; set; }
        public DbSet<PhotoAlbumComment> PhotoAlbumComments { get; set; }
        public DbSet<PhotoAlbumPhoto> PhotoAlbumPhotos { get; set; }
        public DbSet<PhotoFolder> PhotoFolders { get; set; }
        public DbSet<PhotoFolderComment> PhotoFolderComments { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Snippet> Snippets { get; set; }
        public DbSet<SnippetComment> SnippetComments { get; set; }
        public DbSet<SnippetPhoto> SnippetPhotos { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<StoryChapter> StoryChapters { get; set; }
        public DbSet<StoryChapterComment> StoryChapterComments { get; set; }
        public DbSet<StoryChapterPhoto> StoryChapterPhotos { get; set; }
        public DbSet<StoryComment> StoryComments { get; set; }
        public DbSet<StoryPhoto> StoryPhotos { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripComment> TripComments { get; set; }
        public DbSet<TripGroup> TripGroups { get; set; }
        public DbSet<TripPhoto> TripPhotos { get; set; }
        public DbSet<TripSection> TripSections { get; set; }
        public DbSet<TripSubSection> TripSubSections { get; set; }
        public DbSet<UnitHistory> Units { get; set; }
        public DbSet<UnitComment> UnitComments { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<VideoComment> VideoComments { get; set; }
        public DbSet<WhoAmI> WhoAmI { get; set; }
        public DbSet<WhoAmIComment> WhoAmIComments { get; set; }
        public DbSet<WorkEmail> WorkEmails { get; set; }
        public DbSet<WorkOptIn> WorkOptIn { get; set; }
    }
}
