namespace ColbyRJ.DTOs
{
    public class TripCreateDTO : BaseCreateDTO
    {
    }

    public class TripDTO : BaseDTO
    {
        public int YearMonthInt { get; set; }

        public string Note { get; set; } = string.Empty;

        public int YearInt { get; set; }
        public int MonthInt { get; set; }

        [YearValidator(YearStr = "YearStr")]
        public string YearStr { get; set; }

        [MonthValidator(MonthStr = "MonthStr")]
        public string MonthStr { get; set; }
        public string MonStr
        {
            get
            {
                switch (MonthInt)
                {
                    case 1:
                        return "Jan";
                    case 2:
                        return "Feb";
                    case 3:
                        return "Mar";
                    case 4:
                        return "Apr";
                    case 5:
                        return "May";
                    case 6:
                        return "Jun";
                    case 7:
                        return "Jul";
                    case 8:
                        return "Aug";
                    case 9:
                        return "Sep";
                    case 10:
                        return "Oct";
                    case 11:
                        return "Nov";
                    case 12:
                        return "Dec";
                    default:
                        return "";
                }
            }
            set { }
        }
        public string YearMon
        {
            get
            {
                if (YearInt > 0 && MonthInt > 0)
                {
                    return MonStr + " " + YearInt.ToString();
                }
                else 
                {
                    return YearInt.ToString();
                }
            }
            set { }
        }


        [Display(Name = "Photos")]
        public string PhotoCount { get; set; } = string.Empty;
        [Display(Name = "Comments")]
        public string CommentCount { get; set; } = string.Empty;

        public ICollection<TripGroupDTO>? TripGroups { get; set; }
        public ICollection<TripPhotoDTO>? TripPhotos { get; set; }
        public ICollection<TripCommentDTO>? Comments { get; set; }
        public List<string> PhotoUrls { get; set; }
    }
}
