using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class StepTaskDTO
    {
        public int Id { get; set; }

        public string Step { get; set; }

        public int PlanId { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? Schedule { get; set; }

        public int? RemindIn { get; set; }

        public StepTaskStatus Status { get; set; }
    }

    public enum StepTaskStatus
    {
        Completed,
        OverdueCompleted,
        OverdueUnCompleted,
        Uncompleted
    }
}
