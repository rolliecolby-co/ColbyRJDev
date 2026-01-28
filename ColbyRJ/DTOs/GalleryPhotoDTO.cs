namespace ColbyRJ.DTOs
{
    public class GalleryPhotoDTO
    {
        public int Id { get; set; }

        [Required]
        public string Caption { get; set; } = string.Empty;


        [Display(Name = "Order #")]
        public int OrderBy { get; set; }

        public int PhotoYearInt { get; set; }

        public int PhotoMonthInt { get; set; }

        [MonthValidator(MonthStr = "MonthStr")]
        public string MonthStr { get; set; }

        [YearValidator(YearStr = "YearStr")]
        public string YearStr { get; set; }

        public string MonStr
        {
            get
            {
                switch (PhotoMonthInt)
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
                if (PhotoYearInt > 0 && PhotoMonthInt > 0)
                {
                    return MonStr + " " + PhotoYearInt.ToString();
                }
                else if (PhotoYearInt > 0)
                {
                    return PhotoYearInt.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            set { }
        }

        public string Filename { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;

        [Column(TypeName = "Date")]
        public DateTime DateUpdated { get; set; }

        [Required]
        [IntNotZeroValidator(ValueStr = "Gallery Section")]
        public int GallerySectionId { get; set; }
        public GallerySectionDTO Section { get; set; }

        [Required]
        [IntNotZeroValidator(ValueStr = "Decade")]
        public int GalleryDecadeId { get; set; }
        public GalleryDecadeDTO Decade { get; set; }

    }
}
