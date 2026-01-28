using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColbyRJ.Models
{
    public class PhotoFolderComment : BaseCommentEntity
    {

        public int PhotoFolderId { get; set; }
        public PhotoFolder PhotoFolder { get; set; }
    }
}
