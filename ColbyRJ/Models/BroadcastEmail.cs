namespace ColbyRJ.Models
{
    public class BroadcastEmail
    {
        public int Id { get; set; }
        public string SentTo { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string SentBy { get; set; }

        [Column(TypeName = "Date")]
        public DateTime DateSent { get; set; }
    }
}
