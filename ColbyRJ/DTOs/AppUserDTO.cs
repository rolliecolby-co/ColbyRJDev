using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColbyRJ.DTOs
{
    public class AppUserCreateDTO
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;
        [Display(Name = "Display Couple")]
        public string DisplayCouple { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class AppUserDTO : AppUserCreateDTO
    {
        public int Id { get; set; }

        [Display(Name = "Mobile Phone")]
        public string MobilePhone { get; set; } = string.Empty;

        [Display(Name = "Mobile Phone")]
        public string MobilePhoneDisplay
        {
            get
            {
                if (MobilePhone?.Length == 10)
                {
                    return MobilePhone.Substring(0, 3) + "." + MobilePhone.Substring(3, 3) + "." + MobilePhone.Substring(6, 4);
                }
                else
                {
                    return "";
                }
            }
            set { }
        }

        [Display(Name = "Home Phone")]
        public string HomePhone { get; set; } = string.Empty;

        [Display(Name = "Home Phone")]
        public string HomePhoneDisplay
        {
            get
            {
                if (HomePhone?.Length == 10)
                {
                    return HomePhone.Substring(0, 3) + "." + HomePhone.Substring(3, 3) + "." + HomePhone.Substring(6, 4);
                }
                else
                {
                    return "";
                }
            }
            set { }
        }

        public string Address { get; set; } = string.Empty;
        public string Hobbies { get; set; } = string.Empty;
        public string Interests { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string PreviousRole { get; set; } = string.Empty;

        [Display(Name = "Info Opt-In")]
        public bool InfoOptIn { get; set; } = true;

        public DateTime? DOB { get; set; }
        public string DOBDisplay
        {
            get
            {
                if (DOB != null)
                {
                    var strDOB = DOB.ToString();
                    return Convert.ToDateTime(strDOB).ToString("M/d/yyyy");
                }
                else
                {
                    return "";
                }
            }
            set { }
        }
        public string Birthday
        {
            get
            {
                if (DOB != null)
                {
                    var strDOB = DOB.ToString();
                    return Convert.ToDateTime(strDOB).ToString("MMM d");
                }
                else
                {
                    return "";
                }
            }
            set { }
        }
        public DateTime? WeddingDate { get; set; }
        public string WeddingDateDisplay
        {
            get
            {
                if (WeddingDate != null)
                {
                    var strWeddingDate = WeddingDate.ToString();
                    return Convert.ToDateTime(strWeddingDate).ToString("M/d/yyyy");
                }
                else
                {
                    return "";
                }
            }
            set { }
        }
        public string Anniversary
        {
            get
            {
                if (WeddingDate != null)
                {
                    var strWD = WeddingDate.ToString();
                    return Convert.ToDateTime(strWD).ToString("MMM d");
                }
                else
                {
                    return "";
                }
            }
            set { }
        }
        public string EmailConfirmed { get; set; } = string.Empty;
    }
}
