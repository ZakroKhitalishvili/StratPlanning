using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class TextAnswerDTO
    {
        [MaxLength(EntityConfigs.TextAreaMaxLength)]
        public string Text { get; set; }
    }
}
