using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class UserStepResult
    {

        public UserStepResult()
        {
            SelectAnswers = new HashSet<SelectAnswer>();
            BooleanAnswers = new HashSet<BooleanAnswer>();
            IssueOptionAnswers = new HashSet<IssueOptionAnswer>();
            PreparingAnswers = new HashSet<PreparingAnswer>();
            StakeholderRatingAnswers = new HashSet<StakeholderRatingAnswer>();
            StrategicIssueAnswers = new HashSet<StrategicIssueAnswer>();
            TextAnswers = new HashSet<TextAnswer>();
            FileAnswers = new HashSet<FileAnswer>();
            ValueAnswers = new HashSet<ValueAnswer>();
            StepTaskAnswers = new HashSet<StepTaskAnswer>();
        }

        public int Id { get; set; }

        public int? UserToPlanId { get; set; }

        public int? PlanId { get; set; }

        public string Step { get; set; }

        public bool IsSubmitted { get; set; }

        public bool IsDefinitive { get; set; }

        public bool? IsFinal { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual UserToPlan UserToPlan { get; set; }

        public virtual Plan Plan { get; set; }

        public virtual ICollection<BooleanAnswer> BooleanAnswers { get; set; }

        public virtual ICollection<IssueOptionAnswer> IssueOptionAnswers { get; set; }

        public virtual ICollection<PreparingAnswer> PreparingAnswers { get; set; }

        public virtual ICollection<StakeholderRatingAnswer> StakeholderRatingAnswers { get; set; }

        public virtual ICollection<StrategicIssueAnswer> StrategicIssueAnswers { get; set; }

        public virtual ICollection<TextAnswer> TextAnswers { get; set; }

        public virtual ICollection<SelectAnswer> SelectAnswers { get; set; }

        public virtual ICollection<FileAnswer> FileAnswers { get; set; }

        public virtual ICollection<StepTaskAnswer> StepTaskAnswers { get; set; }

        public virtual ICollection<ValueAnswer> ValueAnswers { get; set; }
    }
}
