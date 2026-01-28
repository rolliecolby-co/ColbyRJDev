namespace ColbyRJ.DTOs
{
    public class BasePhotoDTO
    {
        public int Id { get; set; }

        public string Caption { get; set; } = string.Empty;

        [Display(Name = "Order #")]
        public int? OrderBy { get; set; }

        public DateTime? PhotoDate { get; set; }
        public string PhotoDateStr
        {
            get
            {
                if (PhotoDate != null)
                {
                    var photoDate = Convert.ToDateTime(PhotoDate);
                    var photoDateStr = photoDate.ToString("MMM yyyy");
                    return photoDateStr;
                }
                else
                {
                    return "";
                }
            }
            set { }
        }
        public string PhotoUrl { get; set; } = string.Empty;

        public DateTime DateUpdated { get; set; }
    }
}
