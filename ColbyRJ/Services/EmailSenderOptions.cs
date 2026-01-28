namespace ColbyRJ.Services
{
    public class EmailSenderOptions
    {
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string Smtp { get; set; }
        public string AuthID { get; set; }
        public string AuthPwd { get; set; }
    }
}
