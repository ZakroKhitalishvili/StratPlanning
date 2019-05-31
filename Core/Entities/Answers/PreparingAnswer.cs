using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Entities
{
    public class PreparingAnswer : AbstractAnswer
    {
        public int IssueOptionAnswerId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string HowItWillBeDone { get; set; }

        public bool IsCompleted { get; set; }

        public virtual IssueOptionAnswer IssueOptionAnswer { get; set; }

    }
}
