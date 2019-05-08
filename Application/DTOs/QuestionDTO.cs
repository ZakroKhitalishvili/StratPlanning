using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class QuestionDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        
        public string Type { get; set; }

        public int Order { get; set; }

        public bool CanSpecifyOther { get; set; }

        public IList<OptionDTO> Options { get; set; }

        public AnswerDTO Answer { get; set; }

        public IEnumerable<AnswerDTO> OtherAnswers { get; set; }

    }
}
