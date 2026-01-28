using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColbyRJ.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string DisplayCouple { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;       
        public string HomePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Hobbies { get; set; } = string.Empty;
        public string Interests { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public bool InfoOptIn { get; set; } = false;

        [Column(TypeName = "Date")]
        public DateTime? DOB { get; set; }
        [NotMapped]
        public DateTime? Birthday { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? WeddingDate { get; set; }
        [NotMapped]
        public DateTime? AnniversaryDate { get; set; }
    }
}
