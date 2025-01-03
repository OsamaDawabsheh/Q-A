using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.DTOs.Comment
{
    public class GetRepliesDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int QuestionId { get; set; }
        public int UserId { get; set; }

        public int AnswerID { get; set; }

        public int? ParentCommentId  { get; set; }
    }
}
