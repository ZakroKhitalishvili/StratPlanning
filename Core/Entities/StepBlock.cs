using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class StepBlock
    {
        public StepBlock()
        {
            Questions = new HashSet<Question>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Step { get; set; }

        public string Description { get; set; }

        public string Instruction { get; set; }

        public int Order { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

    }
}
