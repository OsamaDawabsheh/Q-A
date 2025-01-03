using ProjectAPI_Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.DTOs.Comment
{
    public class AddCommentDto
    {
        public string Content { get; set; }

        public int UserId { get; set; }

        public int QuestionId { get; set; }

        public int AnswerId { get; set; }

        public int? ParentCommentId { get; set; }

    }
}
