namespace ColbyRJ.Repository
{
    public class TopicRepository : ITopicRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IMapper _mapper;

        public TopicRepository(
            IDbContextFactory<ApplicationDbContext> ctxFactory,
            IMapper mapper)
        {
            _ctxFactory = ctxFactory;
            _mapper = mapper;
        }

        public async Task<string> Create(TopicAddDTO topicDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var topic = new Topic
            {
                Name = topicDTO.Name,
                SectionId = topicDTO.SectionId
            };

            ctx.Topics.Add(topic);
            await ctx.SaveChangesAsync();

            return "id-" + topic.Id.ToString();
        }

        public async Task<int> Delete(int topicId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var topic = await ctx.Topics.FirstOrDefaultAsync(t => t.Id == topicId);

            ctx.Topics.Remove(topic);
            return await ctx.SaveChangesAsync();
        }

        public async Task<List<string>> GetSectionTopicsStr(string section)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var vSection = await ctx.Sections
                .Include(s => s.Topics)
                .Where(s => s.Name == section)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            List<Topic> topics = new List<Topic>();

            List<string> result = new List<string>();

            if (vSection != null && vSection.Topics != null && vSection.Topics.Count > 0)
            {
                topics = vSection.Topics.OrderBy(t => t.Name).ToList();

                topics.ForEach(t =>
                {
                    result.Add(t.Name);
                });
            }

            return result;
        }

        public async Task<TopicDTO> GetTopic(int topicId)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var topic = await ctx.Topics
                .Include(t => t.Section)
                .ThenInclude(s => s.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == topicId);

            if (topic == null)
            {
                return null;
            }

            var topicDTO = _mapper.Map<Topic, TopicDTO>(topic);

            return topicDTO;
        }

        public async Task<List<TopicDTO>> GetTopics()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var topics = await ctx.Topics
                .AsNoTracking()
                .Include(t => t.Section)
                .ThenInclude(s => s.Category)
                .OrderBy(t => t.Section.Name).ThenBy(t => t.Name)
            .ToListAsync();

            var topicsDTO = _mapper.Map<List<Topic>, List<TopicDTO>>(topics);

            return topicsDTO;
        }

        public async Task<string> Update(TopicDTO topicDTO)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var topic = await ctx.Topics
                .FirstOrDefaultAsync(t => t.Id == topicDTO.Id);

            topic.Name = topicDTO.Name;
            topic.SectionId = topicDTO.SectionId;

            ctx.Topics.Update(topic);
            await ctx.SaveChangesAsync();

            return "ok";
        }
    }
}
