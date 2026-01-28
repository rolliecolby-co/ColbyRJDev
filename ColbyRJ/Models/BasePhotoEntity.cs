namespace ColbyRJ.Models
{
    public class BasePhotoEntity
    {
        public int Id { get; set; }

        public string Caption { get; set; } = string.Empty;

        public int? OrderBy { get; set; }


        [Column(TypeName = "Date")]
        public DateTime? PhotoDate { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;

        [Column(TypeName = "Date")]
        public DateTime DateUpdated { get; set; }
    }
}
