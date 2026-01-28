namespace ColbyRJ.DTOs
{
    public class SectionAddDTO
    {
        [Required]
        [Display(Name = "Section")]
        public string Name { get; set; } = string.Empty;

        [RequiredGreaterThanZero]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public CategoryDTO Category { get; set; } = new CategoryDTO();
    }

    public class SectionDTO : SectionAddDTO
    {
        public int Id { get; set; }

        public ICollection<TopicDTO>? Topics { get; set; }
    }
}
