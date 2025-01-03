using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.DTOs.Comment
{
    public class EditCommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public int UserId { get; set; }
    }
}
