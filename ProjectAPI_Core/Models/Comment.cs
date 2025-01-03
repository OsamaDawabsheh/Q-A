using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; }


        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [ForeignKey(nameof(Question))]
        public int QuestionId { get; set; }

        [ForeignKey(nameof(Answer))]
        public int AnswerId { get; set; }

        [ForeignKey(nameof(parentComment))]
        public int? ParentCommentId { get; set; }
        public User user { get; set; }
        public Question question { get; set; }

        public Answer answer { get; set; }

        public Comment parentComment { get; set; }

        public ICollection<Comment> replies { get; set; } = new HashSet<Comment>();
    }
}
