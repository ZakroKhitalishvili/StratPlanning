using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class ValueAnswerDTO
    {
        public int Id { get; set; }

        [MaxLength(EntityConfigs.TextMaxLength)]
        public string Value { get; set; }

        [MaxLength(EntityConfigs.TextAreaMaxLength)]
        public string Definition { get; set; }

        [MaxLength(EntityConfigs.TextAreaMaxLength)]
        public string Description { get; set; }
    }
}
