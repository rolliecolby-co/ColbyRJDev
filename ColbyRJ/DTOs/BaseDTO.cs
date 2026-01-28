using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ColbyRJ.DTOs
{
    public abstract class BaseCreateDTO
    {
        [Required]
        public string Title { get; set; }
        public string Key { get; set; } = string.Empty;

        [Display(Name = "Year")]
        public string YearStr { get; set; } = string.Empty;

        [Display(Name = "Year")]
        [Range(1910, 2050, ErrorMessage = "Please enter valid Year")]
        public int YearInt { get; set; } = DateTime.Now.Year;
        [Display(Name = "Mon")]
        public string MonStr { get; set; } = string.Empty;
        [Display(Name = "Decade")]
        public string Decade { get; set; } = string.Empty;
    }

    public abstract class BaseDTO : BaseCreateDTO
    {
        public int Id { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;

        [Display(Name = "Grouped By")]
        public string GroupedBy { get; set; } = string.Empty;

        [Display(Name = "Year Mon")]
        public string YearMon { get; set; } = string.Empty;
        [Display(Name = "Year Month")]
        public string YearMonth { get; set; } = string.Empty;

        public string Element { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;

        [Display(Name = "Owner Email")]
        public string OwnerEmail { get; set; } = string.Empty;
        public bool Active { get; set; } = false;
        public string ActiveStr { get; set; } = string.Empty;

        [Display(Name = "Date Updated")]
        public DateTime DateUpdated { get; set; }
    }
}
