using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColbyRJ.Models
{
    public class PhotoAlbumPhoto : BasePhotoEntity
    {

        public int PhotoAlbumId { get; set; }
        public PhotoAlbum PhotoAlbum { get; set; }
    }
}
