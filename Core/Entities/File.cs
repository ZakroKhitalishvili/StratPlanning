using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class File
    {
        public File()
        {
            FileAnswers = new HashSet<FileAnswer>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Ext { get; set; }

        public int? QuestionId { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public virtual Question Question { get; set; }

        public virtual Introduction Introduction { get; set; }

        public virtual ICollection<FileAnswer> FileAnswers { get; set; }
    }
}
