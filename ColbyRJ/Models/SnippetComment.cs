using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColbyRJ.Models
{
    public class SnippetComment : BaseCommentEntity
    {

        public int SnippetId { get; set; }
        public Snippet Snippet { get; set; }
    }
}
