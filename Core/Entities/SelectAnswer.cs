using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class SelectAnswer : AbstractAnswer
    {
        public int? OptionId { get; set; }

        public int? IssueId { get; set; }

        public string AltOption { get; set; }

        public virtual Option Option { get; set; }
    }
}
