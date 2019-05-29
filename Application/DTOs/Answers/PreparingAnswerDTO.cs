using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class PreparingAnswerDTO
    {
        public int IssueOptionAnswerId { get; set; }

        public DateTime Date { get; set; }

        public string HowItWillBeDone { get; set; }

        public bool IsCompleted { get; set; }

        public string IssueName { get; set; }

    }
}
