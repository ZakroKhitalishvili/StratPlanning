using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Resource
    {
        public Resource()
        {
            IssueOptionAnswersToResources = new HashSet<IssueOptionAnswerToResource>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public virtual ICollection<IssueOptionAnswerToResource> IssueOptionAnswersToResources { get; set; }

        public virtual BooleanAnswer BooleanAnswer { get; set; }
    }
}
