using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.Models
{
    public class AnswerUsefulness
    {
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [ForeignKey(nameof(Answer))]
        public int AnswerId { get; set; }

        public bool IsUseful { get; set; }

        public User user { get; set; }
        public Answer Answer { get; set; }
    }
}
