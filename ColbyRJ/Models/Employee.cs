using ColbyRJ.Models.CustomValidators;

namespace ColbyRJ.Models
{
    public class Employee
    {
        [Required]
        [EmailDomainValidator(AllowedDomain = "colbyrj.us")]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [RemarksValidator(Remarks = "Employee Remarks")]
        public string Remarks { get; set; }
    }
}
