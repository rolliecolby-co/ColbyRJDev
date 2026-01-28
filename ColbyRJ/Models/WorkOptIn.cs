namespace ColbyRJ.Models
{
    public class WorkOptIn
    {
        public int Id { get; set; }
        public string Email { get; set; } = String.Empty;
        public bool Sent { get; set; } = false;
        public DateTime RefreshDate { get; set; } = DateTime.Today;
    }
}
