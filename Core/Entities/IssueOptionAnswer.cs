using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class IssueOptionAnswer : AbstractAnswer
    {
        public IssueOptionAnswer()
        {
            UserActors = new HashSet<UserToIssueOptionAnswer>();

            Resources = new HashSet<IssueOptionAnswerToResource>();
        }

        public int IssueId { get; set; }

        public string Option { get; set; }

        public bool IsBestOption { get; set; }

        public string Actors { get; set; }

        public virtual ICollection<UserToIssueOptionAnswer> UserActors { get; set; }

        public virtual ICollection<IssueOptionAnswerToResource> Resources { get; set; }

        public virtual TextAnswer Issue { get; set; }
    }
}
