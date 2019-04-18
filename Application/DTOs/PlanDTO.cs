using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class PlanDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsWithActionPlan { get; set; }

        public bool IsCompleted { get; set; }

    }
}
