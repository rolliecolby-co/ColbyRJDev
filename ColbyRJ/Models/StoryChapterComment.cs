using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColbyRJ.Models
{
    public class StoryChapterComment : BaseCommentEntity
    {

        public int StoryChapterId { get; set; }
        public StoryChapter StoryChapter { get; set; }
    }
}
