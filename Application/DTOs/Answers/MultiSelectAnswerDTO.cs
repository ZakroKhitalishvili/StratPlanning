using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class MultiSelectAnswerDTO
    {
        public IList<int> SelectAnswers { get; set; }

        public int? IssueId {get;set;}
    }
}
