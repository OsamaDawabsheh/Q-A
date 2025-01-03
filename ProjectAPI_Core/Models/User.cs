using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.Models
{
    public class User : IdentityUser<int>
    {
        public string ImgName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Question> questions { get; set; } = new HashSet<Question>();

        public ICollection<QuestionUsefulness> questionUsefulnesses { get; set; } = new HashSet<QuestionUsefulness>();

        public ICollection<Answer> answer { get; set; } = new HashSet<Answer>();
        public ICollection<AnswerUsefulness> answerUsefulnesses { get; set; } = new HashSet<AnswerUsefulness>();

    }
}
