﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.DTOs.Answer
{
    public class AddAnswerDto
    {

        public string Content { get; set; }

        public int QuestionId { get; set; }

        public int UserId { get; set; }

    }
}
