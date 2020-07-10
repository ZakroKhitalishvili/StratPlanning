using Core.Constants;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class IssueDistinguishAnswerDTO
    {
        public IssueDistinguishAnswerDTO()
        {

        }
        public int IssueId { get; set; }

        public int QuestionId { get; set; }

        [MaxLength(EntityConfigs.TextAreaMaxLength, ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateMaxStringLength")]

        public string Issue { get; set; }

        public IList<int> SelectAnswers { get; set; }

        public int? SelectAnswer { get; set; }
    }
}
