using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class UserToPlan
    {
        public UserToPlan()
        {
            UserStepResults = new HashSet<UserStepResult>();

            StepTaskAnswers = new HashSet<StepTaskAnswer>();
        }

        public int Id { get; set; }

        public int UserId { get; set; }

        public int PlanId { get; set; }

        public int? PositionId { get; set; }

        public virtual User User { get; set; }

        public virtual Plan Plan { get; set; }

        public virtual DateTime CreatedAt { get; set; }

        public virtual int? CreatedBy { get; set; }

        public virtual ICollection<UserStepResult> UserStepResults { get; set; }

        public virtual Dictionary Position { get; set; }

        public virtual ICollection<StepTaskAnswer> StepTaskAnswers { get; set; }

    }
}
