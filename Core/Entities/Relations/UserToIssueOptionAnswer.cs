using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class UserToIssueOptionAnswer
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int IssueOptionAnswerId { get; set; }

        public virtual User User { get; set; }

        public virtual IssueOptionAnswer IssueOptionAnswer { get; set; }
    }
}
