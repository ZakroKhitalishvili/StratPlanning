using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class SelectAnswerDTO 
    {
        public int? OptionId { get; set; }

        public int? IssueId { get; set; }

        [MaxLength(EntityConfigs.TextMaxLength)]
        public string AltOption { get; set; }
    }
}
