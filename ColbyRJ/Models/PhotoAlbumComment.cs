using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColbyRJ.Models
{
    public class PhotoAlbumComment : BaseCommentEntity
    {

        public int PhotoAlbumId { get; set; }
        public PhotoAlbum PhotoAlbum { get; set; }
    }
}
