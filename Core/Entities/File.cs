using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class File
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Ext { get; set; }

        public int? QuestionId { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public virtual Question Question { get; set; }
    }
}
