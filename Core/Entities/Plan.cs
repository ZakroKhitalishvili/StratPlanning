using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Plan
    {
        public Plan()
        {
            UsersToPlans = new HashSet<UserToPlan>();

            Introductions = new HashSet<Introduction>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsWithActionPlan { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public virtual ICollection<UserToPlan> UsersToPlans { get; set; }

        public virtual ICollection<Introduction> Introductions { get; set; }
    }
}
