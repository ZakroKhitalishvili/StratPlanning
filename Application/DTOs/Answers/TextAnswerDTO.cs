using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Resources;

namespace Application.DTOs
{
    public class TextAnswerDTO
    {
        [MaxLength(EntityConfigs.TextAreaMaxLength, ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateMaxStringLength")]

        public string Text { get; set; }
    }
}
