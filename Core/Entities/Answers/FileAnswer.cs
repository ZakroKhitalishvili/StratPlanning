using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class FileAnswer : AbstractAnswer
    {
        public int FileId { get; set; }

        public virtual File File { get; set; }
    }
}
