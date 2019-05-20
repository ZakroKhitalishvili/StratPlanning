using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class AnswerDTO
    {
        public string Author { get; set; }

        public bool BooleanAnswer { get; set; }

        public SelectAnswerDTO SelectAnswer { get; set; }

        public IList<int> SelectAnswers { get; set; }

        public IList<string> TagSelectAnswers { get; set; }

        public TextAnswerDTO TextAnswer { get; set; }

        public IList<StepTaskAnswerDTO> StepTaskAnswers { get; set; }
    }
}
