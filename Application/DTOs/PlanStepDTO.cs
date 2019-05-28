using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class PlanStepDTO
    {
        [Required]
        public int PlanId { get; set; }

        [Required]
        public string Step { get; set; }

        public IList<StepBlockDTO> StepBlocks { get; set; }

        public IList<QuestionDTO> AdditionalQuestions { get; set; }

        public IList<AnswerGroupDTO> AnswerGroups { get; set; }

        public IList<AnswerGroupDTO> FilledAnswers { get; set; }

        public AnswerGroupDTO StepTaskAnswers { get; set; }

        public IEnumerable<UserPlanningMemberDTO> PlanningTeam { get; set; }

        public IEnumerable<UserPlanningMemberDTO> SubmittedUsers { get; set; }

        public IEnumerable<UserPlanningMemberDTO> NotSubmittedUsers { get; set; }

        public IList<StepTaskDTO> StepTasks { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsSubmitted { get; set; }

    }
}
