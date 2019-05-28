using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class SelectAnswerDTO 
    {
        public int? OptionId { get; set; }

        public int? IssueId { get; set; }

        public string AltOption { get; set; }
    }
}
