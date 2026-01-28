using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColbyRJ.Models
{
    public class StoryChapterPhoto : BasePhotoEntity
    {

        public int StoryChapterId { get; set; }
        public StoryChapter Chapter { get; set; }
    }
}
