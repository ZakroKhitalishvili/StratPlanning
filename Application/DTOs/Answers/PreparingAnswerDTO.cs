using Core.Constants;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class PreparingAnswerDTO
    {
        public int IssueOptionAnswerId { get; set; }

        public DateTime? Date { get; set; }

        [MaxLength(EntityConfigs.TextAreaMaxLength, ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateMaxStringLength")]

        public string HowItWillBeDone { get; set; }

        public bool IsCompleted { get; set; }

        public string IssueName { get; set; }

    }
}
