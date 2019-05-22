using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class StepResponsible
    {
        public int Id { get; set; }

        public string Step { get; set; }

        public int UserToPlanId { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public virtual UserToPlan UserToPlan { get; set; }
    }
}
