using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class StepTask
    {
        public StepTask()
        {
            StepTaskAnswers = new HashSet<StepTaskAnswer>();
        }

        public int Id { get; set; }

        public string Step { get; set; }

        public int PlanId { get; set; }

        public DateTime? Schedule { get; set; }

        public int? Remind { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual Plan Plan { get; set; }

        public virtual ICollection<StepTaskAnswer> StepTaskAnswers { get; set; }

    }
}
