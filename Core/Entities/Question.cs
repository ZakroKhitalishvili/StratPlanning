using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Question
    {
        public Question()
        {
            Files = new HashSet<File>();
            Options = new HashSet<Option>();
            SelectAnswers = new HashSet<SelectAnswer>();
            BooleanAnswers = new HashSet<BooleanAnswer>();
            IssueOptionAnswers = new HashSet<IssueOptionAnswer>();
            PreparingAnswers = new HashSet<PreparingAnswer>();
            StakeholderRatingAnswers = new HashSet<StakeholderRatingAnswer>();
            UserStepResults = new HashSet<UserStepResult>();
            StrategicIssueAnswers = new HashSet<StrategicIssueAnswer>();
            TextAnswers = new HashSet<TextAnswer>();
        }

        public int Id { get; set; }

        public int StepBlockId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public bool HasOptions { get; set; }

        public bool HasFiles { get; set; }

        public int Order { get; set; }

        public bool CanSpecifyOther { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual StepBlock  StepBlock { get; set; }

        public virtual ICollection<Option> Options { get; set; }

        public virtual ICollection<File> Files { get; set; }

        public virtual ICollection<BooleanAnswer> BooleanAnswers { get; set; }

        public virtual ICollection<IssueOptionAnswer> IssueOptionAnswers { get; set; }

        public virtual ICollection<PreparingAnswer> PreparingAnswers { get; set; }

        public virtual ICollection<StakeholderRatingAnswer> StakeholderRatingAnswers { get; set; }

        public virtual ICollection<UserStepResult> UserStepResults { get; set; }

        public virtual ICollection<StrategicIssueAnswer> StrategicIssueAnswers { get; set; }

        public virtual ICollection<TextAnswer> TextAnswers { get; set; }

        public virtual ICollection<SelectAnswer> SelectAnswers { get; set; }

    }
}
