using ProjectAPI_Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.DTOs._َQuestion
{
    public class QuestionUsefulnessDto
    {
        public int QuestionId { get; set; }
        public int UserId { get; set; }

        public bool IsUseful { get; set; }
    }
}
