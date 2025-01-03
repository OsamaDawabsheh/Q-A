using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; }


        [ForeignKey(nameof(User.Id))]
        public int UserId { get; set; }
        public User user { get; set; }
        public ICollection<QuestionUsefulness> questionUsefulnesses { get; set; } = new HashSet<QuestionUsefulness>();

        public ICollection<QuestionTag> questionTags { get; set; } = new HashSet<QuestionTag>();

        public ICollection<Answer> answers { get; set;} = new HashSet<Answer>();

    }
}
