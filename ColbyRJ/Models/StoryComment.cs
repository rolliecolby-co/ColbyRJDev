using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColbyRJ.Models
{
    public class StoryComment : BaseCommentEntity
    {

        public int StoryId { get; set; }
        public Story Story { get; set; }
    }
}
