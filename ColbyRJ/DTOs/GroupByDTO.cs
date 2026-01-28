namespace ColbyRJ.DTOs
{
    public class GroupByDTO
    {
        public string GroupBy { get; set; }
        public int GBCount { get; set; }
    }
    public class GroupByEditDTO
    {
        [Required]
        public string Category { get; set; }
        public string Section { get; set; }
        public string Topic { get; set; }
        public int Id { get; set; }
    }
}
