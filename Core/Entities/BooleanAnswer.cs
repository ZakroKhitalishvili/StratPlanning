using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class BooleanAnswer : AbstractAnswer
    {
        public int? ResourceId { get; set; }

        public bool Answer { get; set; }

        public virtual Resource Resource { get; set; }
    }
}
