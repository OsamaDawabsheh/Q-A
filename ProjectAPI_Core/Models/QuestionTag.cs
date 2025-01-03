using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.Models
{
    public class QuestionTag
    {
        [ForeignKey(nameof(Question))]
        public int QuestionId { get; set; }

        [ForeignKey(nameof(Tag))]
        public int TagId { get; set; }

        public Question question { get; set; }

        public Tag tag { get; set; }

    }
}
