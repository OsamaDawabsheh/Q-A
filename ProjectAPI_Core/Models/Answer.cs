using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.Models
{
    public class Answer
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; }

        public bool IsBest { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [ForeignKey(nameof(Question))]
        public int QuestionId { get; set; }

        public User user { get; set; }
        public Question question { get; set; }

        public ICollection<AnswerUsefulness> answerUsefulnesses { get; set; } = new HashSet<AnswerUsefulness>();
        public ICollection<Comment> comments { get; set; } = new HashSet<Comment>();




    }
}
