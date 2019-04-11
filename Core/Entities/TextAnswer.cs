using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class TextAnswer : AbstractAnswer
    {
        public string Answer { get; set; }

        public bool IsIssue { get; set; }

        public bool IsStakeholder { get; set; }
    }
}
