namespace ColbyRJ.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int? SectionId { get; set; }
        public Section Section { get; set; }
    }
}
