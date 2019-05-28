using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class IssueDistinguishAnswerDTO
    {
        public int IssueId { get; set; }

        public int QuestionId { get; set; }

        public string Issue { get; set; }

        public IList<int> SelectAnswers { get; set; }

        public int SelectAnswer { get; set; }
    }
}
