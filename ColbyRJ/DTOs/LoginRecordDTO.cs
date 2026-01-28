namespace ColbyRJ.DTOs
{
    public class LoginRecordDTO
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public DateTime LoginDate { get; set; }
    }

    public class LoginTallyDTO
    {
        [Display(Name = "Who")]
        public string DisplayName { get; set; }
        public string Email { get; set; }
        [Display(Name = "Number of Logins Last 90 Days")]
        public int NumberOfLoginsLast90Days { get; set; }
        [Display(Name = "Last Login Date")]
        public DateTime LastLoginDate { get; set; }
        [Display(Name = "Last Login Date")]
        public string LastLoginDateStr { get; set; }
    }
}
