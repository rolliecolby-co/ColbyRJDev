namespace ColbyRJ.Models
{
    public class Section
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Topic>? Topics { get; set; }
    }
}
