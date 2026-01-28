using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColbyRJ.Models
{
    public class VideoComment : BaseCommentEntity
    {

        public int VideoId { get; set; }
        public Video Video { get; set; }
    }
}
