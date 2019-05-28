using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class StakeholderRatingAnswer : AbstractAnswer
    {
        public StakeholderRatingAnswer()
        {
            Criteria = new HashSet<StakeholderRatingAnswerToDictionary>();
        }

        public int StakeholderId { get; set; }

        public double? Grade { get; set; }

        public int Priority { get; set; }

        public virtual ICollection<StakeholderRatingAnswerToDictionary> Criteria { get; set; }

        public virtual StakeholderAnswer Stakeholder { get; set; }
    }
}
