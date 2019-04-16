using System;

namespace Core.Entities
{
    public class StepAnswer : AbstractAnswer
    {
        public string Step { get; set; }

        public DateTime Date { get; set; }

        public int Remind { get; set; }

        public bool IsCompleted { get; set; }
    }
}
