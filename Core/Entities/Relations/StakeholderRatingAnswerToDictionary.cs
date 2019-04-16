using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class StakeholderRatingAnswerToDictionary
    {
        public int Id { get; set; }

        public int StakeholderRatingAnswerId { get; set; }

        public int CriterionId { get; set; }

        public int Rate { get; set; }

        public virtual StakeholderRatingAnswer StakeholderRatingAnswer { get; set; }

        public virtual Dictionary Criterion { get; set; }
    }
}
