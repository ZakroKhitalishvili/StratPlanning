using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class AnswerGroupDTO
    {
        public int QuestionId { get; set; }
        
        public AnswerDTO Answer { get; set; }

        public IEnumerable<AnswerDTO> OtherAnswers { get; set; }

        public AnswerDTO DefinitiveAnswer { get; set; }
    }
}
