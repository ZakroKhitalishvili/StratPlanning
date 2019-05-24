using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class SWOTAnswerDTO
    {
        public IList<string> Strengths { get; set; }

        public IList<string> Weaknesses { get; set; }

        public IList<string> Opportunities { get; set; }

        public IList<string> Threats { get; set; }
    }
}
