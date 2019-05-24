using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class SWOTAnswer : AbstractAnswer
    {
        public SWOTAnswer()
        {
            IssueOptionAnswers = new HashSet<IssueOptionAnswer>();
            SelectAnswers = new HashSet<SelectAnswer>();
            StrategicIssueAnswers = new HashSet<StrategicIssueAnswer>();
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool IsIssue { get; set; }

        public virtual ICollection<IssueOptionAnswer> IssueOptionAnswers { get; set; }

        public virtual ICollection<SelectAnswer> SelectAnswers { get; set; }

        public virtual ICollection<StrategicIssueAnswer> StrategicIssueAnswers { get; set; }
    }
}
