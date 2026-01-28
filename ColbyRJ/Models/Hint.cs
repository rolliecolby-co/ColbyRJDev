namespace ColbyRJ.Models
{
    public class Hint
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public int OrderBy { get; set; } = 99;
        public string Title { get; set; }
        public string Value { get; set; }
    }
}
