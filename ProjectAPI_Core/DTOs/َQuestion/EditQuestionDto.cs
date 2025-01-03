using ProjectAPI_Core.DTOs.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.DTOs._َQuestion
{
    public class EditQuestionDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int UserId { get; set; }
        public List<string> Tags { get; set; }
    }
}
