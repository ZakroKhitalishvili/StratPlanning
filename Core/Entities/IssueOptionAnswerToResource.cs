using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class IssueOptionAnswerToResource
    {
        public int IssueOptionAnswerId { get; set; }

        public int ResourceId { get; set; }

        public virtual IssueOptionAnswer IssueOptionAnswer { get; set; }

        public virtual Resource Resource { get; set; }
    }
}
