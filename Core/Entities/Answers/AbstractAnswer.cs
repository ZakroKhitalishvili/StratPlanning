using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public abstract class AbstractAnswer
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public int UserStepResultId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual Question Question { get; set; }

        public UserStepResult UserStepResult { get; set; }
    }
}
