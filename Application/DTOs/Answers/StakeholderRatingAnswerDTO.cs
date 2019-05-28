using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class StakeholderRatingAnswerDTO
    {
        public int StakeholderId { get; set; }

        public double? Grade { get; set; }

        public int Priority { get; set; }

        public string StakeholderName { get; set; }

        public IList<StakeholderRatingByCriterionDTO> CriterionsRates { get; set; }

    }

    public class StakeholderRatingByCriterionDTO
    {
        public int CriterionId { get; set; }

        public int Rate { get; set; }
    }
}
