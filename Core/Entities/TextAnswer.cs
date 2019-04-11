using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class TextAnswer : AbstractAnswer
    {
        TextAnswer()
        {
            IssueOptionAnswers = new HashSet<IssueOptionAnswer>();
        }

        public string Answer { get; set; }

        public bool IsIssue { get; set; }

        public bool IsStakeholder { get; set; }

        public virtual ICollection<IssueOptionAnswer> IssueOptionAnswers { get; set; }
    }
}
