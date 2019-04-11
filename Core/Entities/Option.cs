using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Option
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public virtual Question Question { get;set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

    }
}
