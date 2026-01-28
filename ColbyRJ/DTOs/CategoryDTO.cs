namespace ColbyRJ.DTOs
{
    public class CategoryAddDTO
    {
        [Required]
        [Display(Name = "Category")]
        public string Name { get; set; } = string.Empty;
    }

    public class CategoryDTO : CategoryAddDTO
    {
        public int Id { get; set; }

        public ICollection<SectionDTO>? Sections { get; set; }
    }
}
