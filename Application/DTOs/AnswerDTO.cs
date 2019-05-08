using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class AnswerDTO
    {
        public BooleanAnswerDTO BooleanAnswer { get; set; }

        public SelectAnswerDTO SelectAnswer { get; set; }

        public IList<SelectAnswerDTO> SelectAnswers { get; set; }

        public TextAnswerDTO TextAnswer { get; set; }
    }
}
