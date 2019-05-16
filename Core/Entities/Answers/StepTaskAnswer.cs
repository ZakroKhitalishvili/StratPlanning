using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class StepTaskAnswer
    {
        public int Id { get; set; }

        public int UserToPlanId { get; set; }

        public int StepTaskId { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsDefinitive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual UserToPlan UserToPlan { get; set; }

        public virtual StepTask StepTask { get; set; }

    }
}
