using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class StepBlockDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Instruction { get; set; }

        public int Order { get; set; }

        public IList<QuestionDTO> Questions { get; set; }


    }
}
