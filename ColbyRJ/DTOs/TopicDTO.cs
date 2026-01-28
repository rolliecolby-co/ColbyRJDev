namespace ColbyRJ.DTOs
{
    public class TopicAddDTO
    {
        [Required]
        [Display(Name = "Topic")]
        public string Name { get; set; } = string.Empty;

        [RequiredGreaterThanZero]
        [Display(Name = "Section")]
        public int SectionId { get; set; }
        public SectionDTO Section { get; set; } = new SectionDTO();
    }

    public class TopicDTO : TopicAddDTO
    {
        public int Id { get; set; }
    }
}
