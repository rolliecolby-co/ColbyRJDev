namespace ColbyRJ.DTOs
{
    public class BroadcastEmailDTO
    {
        public int Id { get; set; }
        public string SentTo { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public string SentBy { get; set; }

        public DateTime DateSent { get; set; }

        public string DateSentStr { get; set; }

        public List<string> UserIDs { get; set; }
    }
}
