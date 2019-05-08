using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class StakeholderRatingAnswerDTO
    {
        public int StakeholderId { get; set; }

        public int? Grade { get; set; }

        public int Priority { get; set; }

        //public virtual ICollection<StakeholderRatingAnswerToDictionary> Criteria { get; set; }

    }
}
