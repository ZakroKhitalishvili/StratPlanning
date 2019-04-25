using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class User
    {
        public User()
        {
            UsersToPlans = new HashSet<UserToPlan>();

            UsersToIssueOptionAnswers = new HashSet<UserToIssueOptionAnswer>();
        }

        public int Id { get; set; }

        public int? PositionId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual Dictionary Position { get; set; }

        public virtual ICollection<UserToPlan> UsersToPlans { get; set; }

        public virtual ICollection<UserToIssueOptionAnswer> UsersToIssueOptionAnswers { get; set; }

    }
}
