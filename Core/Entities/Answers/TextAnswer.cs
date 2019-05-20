using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class TextAnswer : AbstractAnswer
    {
        public TextAnswer()
        {
            IssueOptionAnswers = new HashSet<IssueOptionAnswer>();

            SelectAnswers = new HashSet<SelectAnswer>();

            StakeholderRatingAnswers = new HashSet<StakeholderRatingAnswer>();

            StrategicIssueAnswers = new HashSet<StrategicIssueAnswer>();
        }

        public string Text { get; set; }

        public bool IsIssue { get; set; }

        public bool IsStakeholder { get; set; }

        public virtual ICollection<IssueOptionAnswer> IssueOptionAnswers { get; set; }

        public virtual ICollection<SelectAnswer> SelectAnswers { get; set; }

        public virtual ICollection<StakeholderRatingAnswer> StakeholderRatingAnswers { get; set; }

        public virtual ICollection<StrategicIssueAnswer> StrategicIssueAnswers { get; set; }

    }
}
