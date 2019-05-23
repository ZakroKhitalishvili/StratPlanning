using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Dictionary
    {
        public Dictionary()
        {
            Users = new HashSet<User>();

            StakeholderRatingAnswersToDictionaries = new HashSet<StakeholderRatingAnswerToDictionary>();

            UsersToPlans = new HashSet<UserToPlan>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public bool HasPosition { get; set; }

        public bool HasCriterion { get; set; }

        public bool HasValue { get; set; }

        public DateTime CreatedAt { get; set; }
    
        public DateTime UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public virtual ICollection<StakeholderRatingAnswerToDictionary> StakeholderRatingAnswersToDictionaries { get; set; }

        public virtual ICollection<UserToPlan> UsersToPlans { get; set; }
    }
}
