using ProjectAPI_Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.DTOs.Answer
{
    public class BestAnswerDto
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public int UserId { get; set; }

        public bool IsBest { get; set; }


    }
}
