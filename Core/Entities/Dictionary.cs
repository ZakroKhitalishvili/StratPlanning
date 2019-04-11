using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Dictionary
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool HasPosition { get; set; }

        public bool HasCriterion { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

    }
}
