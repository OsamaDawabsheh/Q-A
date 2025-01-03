using ProjectAPI_Core.DTOs.Tag;
using ProjectAPI_Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.DTOs._َQuestion
{
    public class GetQuestionsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int UserId { get; set; }

        public int UsefulCount { get; set; }
        public int UselessCount { get; set; }

        public int AnswersCount { get; set; }

        public List<GetTagsDto> Tags { get; set; }


    }
}
