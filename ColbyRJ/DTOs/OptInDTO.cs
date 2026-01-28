namespace ColbyRJ.DTOs
{
    public class OptInDTO
    {
    }
    public class OptInChangeDTO
    {
        public int OrderBy { get; set; }
        public string WhatChanged { get; set; }
        public string ChangedTitle { get; set; }
    }

    public class OptInSendToDTO
    {
        public string SendToEmail { get; set; }
        public string SendToName { get; set; }
    }

    public class OptInSendDTO
    {
        public string Email { get; set; }

        public DateTime SentDT { get; set; }
    }
}
