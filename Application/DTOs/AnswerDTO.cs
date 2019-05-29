using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class AnswerDTO
    {
        public string Author { get; set; }

        public bool BooleanAnswer { get; set; }

        public SelectAnswerDTO SelectAnswer { get; set; }

        public MultiSelectAnswerDTO MultiSelectAnswer { get; set; }

        public IList<string> TagSelectAnswers { get; set; }

        public TextAnswerDTO TextAnswer { get; set; }

        public IList<FileAnswerDTO> FileAnswer { get; set; }

        public IList<int> InputFileAnswer { get; set; }

        public IList<StepTaskAnswerDTO> StepTaskAnswers { get; set; }

        public IList<ValueAnswerDTO> ValueAnswer { get; set; }

        public IList<StakeholderAnswerDTO> StakeholderAnswers { get; set; }

        public SWOTAnswerDTO SwotAnswer { get; set; }

        public IList<StrategicIssueAnswerDTO> StrategicIssueAnswers { get; set; }

        public IList<StakeholderRatingAnswerDTO> StakeholderRatingAnswers { get; set; }

        public IList<IssueOptionAnsweDTO> IssueOptionAnswers { get; set; }

        public IList<IssueDistinguishAnswerDTO> IssueDistinguishAnswers { get; set; }

        public IList<PreparingAnswerDTO> IssuePreparingAnswers { get; set; }

        public IList<ResourceReviewAnswerDTO> ResourceReviewAnswers { get; set; }
    }
}
