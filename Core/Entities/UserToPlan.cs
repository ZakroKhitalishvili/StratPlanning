using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class UserToPlan
    {
        public UserToPlan()
        {
            SelectAnswers = new HashSet<SelectAnswer>();
            BooleanAnswers = new HashSet<BooleanAnswer>();
            IssueOptionAnswers = new HashSet<IssueOptionAnswer>();
            PreparingAnswers = new HashSet<PreparingAnswer>();
            StakeholderRatingAnswers = new HashSet<StakeholderRatingAnswer>();
            StepAnswers = new HashSet<StepAnswer>();
            StrategicIssueAnswers = new HashSet<StrategicIssueAnswer>();
            TextAnswers = new HashSet<TextAnswer>();
        }

        public int Id { get; set; }

        public int UserId { get; set; }

        public int PlanId { get; set; }

        public virtual User User { get; set; }

        public virtual Plan Plan { get; set; }

        public virtual ICollection<BooleanAnswer> BooleanAnswers { get; set; }

        public virtual ICollection<IssueOptionAnswer> IssueOptionAnswers { get; set; }

        public virtual ICollection<PreparingAnswer> PreparingAnswers { get; set; }

        public virtual ICollection<StakeholderRatingAnswer> StakeholderRatingAnswers { get; set; }

        public virtual ICollection<StepAnswer> StepAnswers { get; set; }

        public virtual ICollection<StrategicIssueAnswer> StrategicIssueAnswers { get; set; }

        public virtual ICollection<TextAnswer> TextAnswers { get; set; }

        public virtual ICollection<SelectAnswer> SelectAnswers { get; set; }

    }
}
