using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class StrategicIssueAnswerDTO
    {
        public int IssueId { get; set; }

        public string Issue { get; set; }

        public string Why { get; set; }

        public string Result { get; set; }

        public string Goal { get; set; }

        public string Solution { get; set; }

        public int Ranking { get; set; }
    }
}
