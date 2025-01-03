using ProjectAPI_Core.DTOs.Comment;
using ProjectAPI_Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.DTOs.Answer
{
    public class GetAnswersDto
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        public bool IsBest { get; set; }

        public int UserId { get; set; }

        public int QuestionId { get; set; }

        public int UsefulCount { get; set; }

        public int UselessCount { get; set; }

        public List<GetCommentsDto> comments { get; set; }


    }
}
