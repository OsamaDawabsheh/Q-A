using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.DTOs.Tag
{
    public class GetTagsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int? QuestionsCount {  get; set; }
    }
}
