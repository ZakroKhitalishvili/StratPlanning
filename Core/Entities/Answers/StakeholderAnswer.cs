using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class StakeholderAnswer : AbstractAnswer
    {
        public StakeholderAnswer()
        {
            StakeholderRatingAnswers = new HashSet<StakeholderRatingAnswer>();
        }

        public int? UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool IsInternal { get; set; }

        public int? CategoryId { get; set; }

        public virtual Dictionary Category { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<StakeholderRatingAnswer> StakeholderRatingAnswers { get; set; }
    }
}
