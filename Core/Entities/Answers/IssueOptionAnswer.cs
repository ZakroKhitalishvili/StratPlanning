using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class IssueOptionAnswer : AbstractAnswer
    {
        public IssueOptionAnswer()
        {
            UsersToIssueOptionAnswers = new HashSet<UserToIssueOptionAnswer>();

            IssueOptionAnswersToResources = new HashSet<IssueOptionAnswerToResource>();

            PreparingAnswers = new HashSet<PreparingAnswer>();
        }

        public int IssueId { get; set; }

        public string Option { get; set; }

        public bool IsBestOption { get; set; }

        public string Actors { get; set; }

        public virtual ICollection<UserToIssueOptionAnswer> UsersToIssueOptionAnswers { get; set; }

        public virtual ICollection<IssueOptionAnswerToResource> IssueOptionAnswersToResources { get; set; }

        public virtual SWOTAnswer Issue { get; set; }

        public virtual ICollection<PreparingAnswer> PreparingAnswers { get; set; }
    }
}
