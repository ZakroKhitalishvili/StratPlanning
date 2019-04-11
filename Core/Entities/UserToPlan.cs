using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class UserToPlan
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int PlanId { get; set; }

        public virtual User User { get; set; }

        public virtual Plan Plan { get; set; }
    }
}
