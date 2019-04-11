using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Question
    {
        public Question()
        {
            Files = new HashSet<File>();
        }

        public int Id { get; set; }

        public int StepBlockId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public bool HasOptions { get; set; }

        public bool HasFiles { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public virtual StepBlock  StepBlock { get; set; }

        public virtual ICollection<File> Files { get; set; }

    }
}
