using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class StrategicIssueAnswer : AbstractAnswer
    {
        public int IssueId { get; set; }

        public string Why { get; set; }

        public string Result { get; set; }

        public string Goal { get; set; }

        public string Solution { get; set; }

        public int Ranking { get; set; }

        public virtual TextAnswer Issue { get; set; }
    }
}
