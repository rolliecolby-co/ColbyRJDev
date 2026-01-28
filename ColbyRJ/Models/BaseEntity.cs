namespace ColbyRJ.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        
        public string Title { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string GroupedBy { get; set; } = string.Empty;

        public string YearStr { get; set; } = string.Empty;
        public string MonStr { get; set; } = string.Empty;
        public string YearMon { get; set; } = string.Empty;
        public string YearMonth { get; set; } = string.Empty;

        public string Element { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public bool Active { get; set; } = false;

        [Column(TypeName = "Date")]
        public DateTime DateUpdated { get; set; }
    }
}
