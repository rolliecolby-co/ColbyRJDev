namespace ColbyRJ.DTOs
{
    public class BrowseDTO
    {
        public List<GroupByDTO> GroupedBy { get; set; }
        public List<GroupByElementDTO> GroupedByElements { get; set; }
        public List<DecadeDTO> Decades { get; set; }
        public List<DecadeElementDTO> DecadeElements { get; set; }

        public List<PhotoAlbum>? PhotoAlbums { get; set; }
        public List<PhotoFolder>? PhotoFolders { get; set; }
        public List<Snippet>? Snippets { get; set; }
        public List<Story>? Stories { get; set; }
        public List<Video>? Videos { get; set; }
    }
}
