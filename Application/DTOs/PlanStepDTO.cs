using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class PlanStepDTO
    {
        public int PlanId { get; set; }

        public string Step { get; set; }

        public IList<StepBlockDTO> StepBlocks { get; set; }

        public IEnumerable<UserPlanningMemberDTO> PlanningTeam { get; set; }

    }
}
