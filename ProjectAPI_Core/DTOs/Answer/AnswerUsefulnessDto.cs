using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.DTOs.Answer
{
    public class AnswerUsefulnessDto
    {

        public int UserId { get; set; }

        public int AnswerId { get; set; }

        public bool IsUseful { get; set; }
    }
}
